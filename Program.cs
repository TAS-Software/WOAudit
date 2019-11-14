using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WOAudit
{
    class Program
    {
        static void Main(string[] args)
        {
            reportThas01Entities thas01dB = new reportThas01Entities();
            //liveThas01Entities liveThas01dB = new liveThas01Entities();
            //LiveConnectDbEntities liveConnectDB = new LiveConnectDbEntities();
            ReportConnectDbEntities reportConnectDB = new ReportConnectDbEntities();
            reportConnectDB.Database.CommandTimeout = 15000;
            thas01dB.Database.CommandTimeout = 15000;

            //liveThas01dB.Database.CommandTimeout = 15000;
            //liveConnectDB.Database.CommandTimeout = 15000;
            var woDateTimeNow = DateTime.Now;
            if (woDateTimeNow.Hour == 0 || woDateTimeNow.Hour >= 5)
            {
                Console.WriteLine("Starting..");
                thas01dB.THAS_CONNECT_CopyLiveWODatasetToTable();
                Console.WriteLine("Finished Live WO Dataset");
                reportConnectDB.THAS_CONNECT_CopyWorksOrderChanges();
                Console.WriteLine("Finished Copy WO Changes");
                reportConnectDB.THAS_CONNECT_CopyWorksOrderPartChanges();
                Console.WriteLine("Finished Copy WOP Changes");

                var woLevelAddAuditList = new List<WOAuditEntry>();
                var woPartLevelAddAuditList = new List<WOAuditEntry>();
                var woLevelDeleteAuditList = new List<WOAuditEntry>();
                var woPartLevelDeleteAuditList = new List<WOAuditEntry>();
                var woLevelAuditList = new List<WOAuditEntry>();
                var woAuditList = new List<WOAuditEntry>();

                var allNewAdditions = reportConnectDB.THAS_CONNECT_WorksOrderAuditAdditions().ToList();
                var allNewWOPAdditions = reportConnectDB.THAS_CONNECT_WorksOrderPartAuditAdditions().ToList();
                var allNewWOs = allNewAdditions.Select(x => x.WorksOrderNumber).Distinct().ToList();

                foreach (var wo in allNewWOs)
                {
                    var entry = allNewAdditions.FirstOrDefault(x => x.WorksOrderNumber == wo);
                    if (entry != null)
                    {
                        var auditEntry = new WOAuditEntry
                        {
                            WOPart = 0,
                            ChangedFrom = "",
                            ChangedTo = wo,
                            FieldName = "WorksOrderNumber",
                            ModifiedOn = woDateTimeNow,
                            WorksOrderNumber = wo,
                            WOLevel = "WO",
                            WOTID = entry.WOTID,
                            Type = "New"
                        };
                        woLevelAddAuditList.Add(auditEntry);
                    }
                   
                }
                if (woLevelAddAuditList.Count() > 0)
                {
                    CopyWOAuditEntriesToDB(woLevelAddAuditList);
                }

                var allNewWOPs = allNewWOPAdditions.Select(x => x.WOPart).Distinct().ToList();

                foreach (var wop in allNewWOPs)
                {
                    var entry = allNewWOPAdditions.FirstOrDefault(x => x.WOPart == wop);
                    if (entry != null)
                    {
                        var isStoresReq = entry.IsStoreRequest.Value ? "Yes" : "No";
                        var auditEntry = new WOAuditEntry
                        {
                            WOPart = entry.WOPart,
                            ChangedFrom = "",
                            ChangedTo = entry.CompPartNumber + " - Seq: " + entry.SequenceNo,
                            FieldName = "ComponentPartNumber",
                            ModifiedOn = woDateTimeNow,
                            WorksOrderNumber = entry.WorksOrderNumber,
                            WOLevel = "WO Part",
                            WOTID = entry.WOTID,
                            Type = "New SR?: " + isStoresReq
                        };
                        woPartLevelAddAuditList.Add(auditEntry);
                    }
                }
                if (woPartLevelAddAuditList.Count() > 0)
                {
                    CopyWOAuditEntriesToDB(woPartLevelAddAuditList);
                }
                Console.WriteLine("Finished Additions");
                var allDeletions = reportConnectDB.THAS_CONNECT_WorksOrderAuditDeletions().ToList();
                var allWODeletions = allDeletions.Select(x => x.WorksOrderNumber).Distinct().ToList();

                foreach (var wo in allWODeletions)
                {
                    var entry = allDeletions.FirstOrDefault(x => x.WorksOrderNumber == wo);
                    if (entry != null)
                    {
                        var auditEntry = new WOAuditEntry
                        {
                            WOPart = 0,
                            ChangedFrom = wo,
                            ChangedTo = "",
                            FieldName = "WorksOrderNumber",
                            ModifiedOn = woDateTimeNow,
                            WorksOrderNumber = wo,
                            WOLevel = "WO",
                            WOTID = entry.WOTID,
                            Type = "Delete"
                        };
                        woLevelDeleteAuditList.Add(auditEntry);
                    }
                }
                if (woLevelDeleteAuditList.Count() > 0)
                {
                    CopyWOAuditEntriesToDB(woLevelDeleteAuditList);
                }

                var allWOPDeletions = allDeletions.Select(x => x.WOPart).Distinct().ToList();

                foreach (var wop in allWOPDeletions)
                {
                    var entry = allDeletions.FirstOrDefault(x => x.WOPart == wop);
                    if (entry != null)
                    {
                        var isStoresReq = entry.IsStoreRequest.Value ? "Yes" : "No";
                        var auditEntry = new WOAuditEntry
                        {
                            WOPart = entry.WOPart,
                            ChangedFrom = entry.CompPartNumber + " - Seq: " + entry.SequenceNo,
                            ChangedTo = "",
                            FieldName = "ComponentPartNumber",
                            ModifiedOn = woDateTimeNow,
                            WorksOrderNumber = entry.WorksOrderNumber,
                            WOLevel = "WO Part",
                            WOTID = entry.WOTID,
                            Type = "Delete SR?: " + isStoresReq
                        };
                        woPartLevelDeleteAuditList.Add(auditEntry);
                    }
                }
                if (woPartLevelDeleteAuditList.Count() > 0)
                {
                    CopyWOAuditEntriesToDB(woPartLevelDeleteAuditList);
                }
                Console.WriteLine("Finished Deletions");
                var changedWORecords = reportConnectDB.WOAuditChanges.Select(x => x.WorksOrderNumber).Distinct().ToList();
                var getOldEntryList = reportConnectDB.WOAuditDumps.Where(x => changedWORecords.Contains(x.WorksOrderNumber)
                    ).ToList();
                var getNewEntryList = reportConnectDB.WOAuditChanges.Where(x => changedWORecords.Contains(x.WorksOrderNumber)
                    ).ToList();

                Console.WriteLine("Retrieved Changed WO Records");

                foreach (var woLevel in changedWORecords)
                {
                    var existingEntry = getOldEntryList.FirstOrDefault(x => x.WorksOrderNumber == woLevel); //
                    var thisRec = getNewEntryList.FirstOrDefault(x => x.WorksOrderNumber == woLevel);
                    if (existingEntry != null && thisRec != null)
                    {
                        if (existingEntry.BatchQuantity != thisRec.BatchQuantity)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.BatchQuantity.ToString(),
                                ChangedTo = thisRec.BatchQuantity.ToString(),
                                FieldName = "WOBatchQuantity",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.QuantityStored != thisRec.QuantityStored)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.QuantityStored.ToString(),
                                ChangedTo = thisRec.QuantityStored.ToString(),
                                FieldName = "WOStoredQuantity",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.CompletionDate != thisRec.CompletionDate)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.CompletionDate.ToShortDateString(),
                                ChangedTo = thisRec.CompletionDate.ToShortDateString(),
                                FieldName = "CompletionDate",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.ActualReceiptDate != thisRec.ActualReceiptDate)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.ActualReceiptDate.HasValue ? existingEntry.ActualReceiptDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "",
                                ChangedTo = thisRec.ActualReceiptDate.HasValue ? thisRec.ActualReceiptDate.Value.ToString("dd-MM-yyyy HH:mm:ss") : "",
                                FieldName = "ActualReceiptDate",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WorksOrderStatus != thisRec.WorksOrderStatus)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.WorksOrderStatus,
                                ChangedTo = thisRec.WorksOrderStatus,
                                FieldName = "WorksOrderStatus",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WOMethod != thisRec.WOMethod)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.WOMethod,
                                ChangedTo = thisRec.WOMethod,
                                FieldName = "MethodType",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WORespCode != thisRec.WORespCode)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.WORespCode,
                                ChangedTo = thisRec.WORespCode,
                                FieldName = "RespCode",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WorksOrderType != thisRec.WorksOrderType)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.WorksOrderType,
                                ChangedTo = thisRec.WorksOrderType,
                                FieldName = "WorksOrderType",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.OnHold != thisRec.OnHold)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.OnHold ? "Yes" : "No",
                                ChangedTo = thisRec.OnHold ? "Yes" : "No",
                                FieldName = "OnHold",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.Exclude != thisRec.Exclude)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.Exclude ? "Yes" : "No",
                                ChangedTo = thisRec.Exclude ? "Yes" : "No",
                                FieldName = "ExcludeMRPSuggestion",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WOIssue != thisRec.WOIssue)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = 0,
                                ChangedFrom = existingEntry.WOIssue,
                                ChangedTo = thisRec.WOIssue,
                                FieldName = "WOIssue",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = thisRec.WorksOrderNumber,
                                WOLevel = "WO",
                                WOTID = thisRec.WOTID,
                                Type = "Change"
                            };
                            woLevelAuditList.Add(auditEntry);
                        }
                    }
                }
                CopyWOAuditEntriesToDB(woLevelAuditList);
                Console.WriteLine("After WO Level");

                var changedWOPRecords = reportConnectDB.WOPartAuditChanges.Select(x => x.WOPart).ToList();
                var getWOPOldEntryList = reportConnectDB.WOAuditDumps.Where(x => changedWOPRecords.Contains(x.WOPart)
                   ).ToList();
                var getWOPNewEntryList = reportConnectDB.WOPartAuditChanges.Where(x => changedWOPRecords.Contains(x.WOPart)
                    ).ToList();

                foreach (var woLine in getWOPNewEntryList)
                {
                    var existingEntry = getWOPOldEntryList.FirstOrDefault(x => x.WOPart == woLine.WOPart);

                    if (existingEntry != null)
                    {
                        if (existingEntry.CompPartID != woLine.CompPartID)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.CompPartNumber,
                                ChangedTo = woLine.CompPartNumber,
                                FieldName = "ComponentPartNumber",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.PlannedIssueQuantity != woLine.PlannedIssueQuantity)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.PlannedIssueQuantity.ToString(),
                                ChangedTo = woLine.PlannedIssueQuantity.ToString(),
                                FieldName = "PlannedIssueQuantity",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.ActualIssueQuantity != woLine.ActualIssueQuantity)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.ActualIssueQuantity.ToString(),
                                ChangedTo = woLine.ActualIssueQuantity.ToString(),
                                FieldName = "ActualIssueQuantity",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.WOCompIssueStatus != woLine.WOCompIssueStatus)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.WOCompIssueStatus,
                                ChangedTo = woLine.WOCompIssueStatus,
                                FieldName = "ComponentIssueStatus",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.SequenceNo != woLine.SequenceNo)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.SequenceNo.ToString(),
                                ChangedTo = woLine.SequenceNo.Value.ToString(),
                                FieldName = "SequenceNo",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.CompIssued != woLine.CompIssued)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.CompIssued.ToString(),
                                ChangedTo = woLine.CompIssued.ToString(),
                                FieldName = "FullyIssued",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                        if (existingEntry.MRPIgnore != woLine.MRPIgnore)
                        {
                            var auditEntry = new WOAuditEntry
                            {
                                WOPart = woLine.WOPart,
                                ChangedFrom = existingEntry.MRPIgnore.ToString(),
                                ChangedTo = woLine.MRPIgnore.ToString(),
                                FieldName = "MRPToIgnore",
                                ModifiedOn = woDateTimeNow,
                                WorksOrderNumber = woLine.WorksOrderNumber,
                                WOLevel = "WOPart",
                                WOTID = woLine.WOTID,
                                Type = "Change"
                            };
                            woAuditList.Add(auditEntry);
                        }
                    }
                }
                try
                {
                    CopyWOAuditEntriesToDB(woAuditList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Copy WO Audit Entries Failed: " + ex.Message + ex.InnerException);
                }

                Console.WriteLine("After WO Component Level");

                using (var newthas = new reportThas01Entities())
                {
                    try
                    {
                        newthas.Database.CommandTimeout = 15000;
                        Console.Write("Begin Inserting New Data To Table - " + DateTime.Now);
                        try
                        {
                            reportConnectDB.Database.ExecuteSqlCommand("truncate table WOAuditDump");
                            Console.WriteLine("Finished Truncating");
                            newthas.THAS_CONNECT_CopyWODatasetToTable();
                            Console.WriteLine("Finished Copy");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed WO Dataset Copy" + ex.Message + ex.InnerException);
                        }

                        using (MailMessage mail = new MailMessage("WOAudit@thompsonaero.com", "Sean.Kelly@thompsonaero.com"))
                        {

                            mail.Subject = "WO Audit Successfully Ran";
                            mail.Body = "Finished as expected.";
                            mail.IsBodyHtml = true;
                            SmtpClient client = new SmtpClient("remote.thompsonaero.com");
                            client.Send(mail);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (MailMessage mail = new MailMessage("WOAudit@thompsonaero.com", "Sean.Kelly@thompsonaero.com"))
                        {

                            mail.Subject = "WO Audit Successfully Ran";
                            mail.Body = "Finished as expected.";
                            mail.IsBodyHtml = true;
                            SmtpClient client = new SmtpClient("remote.thompsonaero.com");
                            client.Send(mail);
                        }
                    }
                }
            }
            
        }
        public static void CopyWOAuditEntriesToDB(List<WOAuditEntry> dataSet)
        {
            ReportConnectDbEntities connect = null;
            try
            {
                connect = new ReportConnectDbEntities();
                connect.Configuration.AutoDetectChangesEnabled = false;

                int count = 0;
                foreach (var line in dataSet)
                {
                    ++count;
                    var record = new WOAuditEntry();
                    record.WOPart = line.WOPart;
                    record.WOLevel = line.WOLevel;
                    record.ModifiedOn = line.ModifiedOn;
                    record.WorksOrderNumber = line.WorksOrderNumber;
                    record.FieldName = line.FieldName;
                    record.ChangedFrom = line.ChangedFrom;
                    record.ChangedTo = line.ChangedTo;
                    record.WOTID = line.WOTID;
                    record.Type = line.Type;


                    connect = AddToContextWOAudit(connect, record, count, 500, true);
                }
                connect.SaveChanges();
            }
            finally
            {
                if (connect != null)
                    connect.Dispose();
            }

        }
        private static ReportConnectDbEntities AddToContextWOAudit(ReportConnectDbEntities context, WOAuditEntry entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<WOAuditEntry>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new ReportConnectDbEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            return context;
        }
    }
    
}
