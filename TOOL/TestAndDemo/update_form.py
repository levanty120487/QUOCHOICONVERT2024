import re

with open('FormCloneMoetVanBanDuThao.cs', 'r', encoding='utf-8') as f:
    code = f.read()

# Replace the VanBanDuThaoItem class fields
class_def_old = """    public class VanBanDuThaoItem
    {
        public string TrichYeu { get; set; }
        public string LoaiVanBan { get; set; }
        public string LinhVuc { get; set; }
        public string SoKyHieu { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public string TinhTrangHieuLuc { get; set; }
        public string DetailUrl { get; set; }

        public string CoQuanBanHanh { get; set; }
        public string NguoiKy { get; set; }
        public DateTime? NgayCoHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }

        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

        public DateTime CreateAt {  get; set; } = DateTime.Now;
    }"""

class_def_new = """    public class VanBanDuThaoItem
    {
        public string TenDuThao { get; set; }
        public string LoaiVanBan { get; set; }
        public string LinhVuc { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public string DetailUrl { get; set; }
        public string NoiDungDuThao { get; set; }

        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }"""
code = code.replace(class_def_old, class_def_new)

# Replace ReadListPageAsync
read_old = """                var listNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-legal-document-table')]//div[contains(@class, 'items')]");
                if (listNodes == null) return result;

                int itemIndex = 0;
                foreach (var itemNode in listNodes)
                {
                    itemIndex++;
                    lblStatus.Text = $"Đã đọc trang {currentPageIndex}/ bản ghi thứ {itemIndex}";
                    Application.DoEvents();

                    var item = new VanBanDuThaoItem();

                    var trichYeuNode = itemNode.SelectSingleNode(".//div[contains(@class, 'border-right') and contains(@class, 'border-dotted')]");
                    if (trichYeuNode != null) item.TrichYeu = CleanText(trichYeuNode.InnerText);

                    var loaiVanBanNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[2]//td[1]");
                    if (loaiVanBanNode != null) item.LoaiVanBan = CleanText(loaiVanBanNode.InnerText);

                    var linhVucNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[3]//td[1]");
                    if (linhVucNode != null) item.LinhVuc = CleanText(linhVucNode.InnerText);

                    var soKyHieuNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[1]//td[1]");
                    if (soKyHieuNode != null) item.SoKyHieu = CleanText(soKyHieuNode.InnerText);

                    var ngayBanHanhNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[1]//td[2]");
                    if (ngayBanHanhNode != null) item.NgayBanHanh = ParseDate(CleanText(ngayBanHanhNode.InnerText));

                    var tinhTrangHieuLucNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[4]//td[1]");
                    if (tinhTrangHieuLucNode != null) item.TinhTrangHieuLuc = CleanText(tinhTrangHieuLucNode.InnerText);

                    var titleLinkNode = itemNode.SelectSingleNode(".//div[contains(@class, 'title')]//a");
                    if (titleLinkNode != null)
                    {
                        item.DetailUrl = ToAbsoluteUrl(pageUrl, titleLinkNode.GetAttributeValue("href", ""));
                    }

                    if (!string.IsNullOrEmpty(item.DetailUrl))
                    {
                        await ParseDetailAsync(httpClient, item, item.DetailUrl);
                    }

                    result.Add(item);
                }"""

read_new = """                var listNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'items padding-xs')]");
                if (listNodes == null) return result;

                int itemIndex = 0;
                foreach (var itemNode in listNodes)
                {
                    itemIndex++;
                    lblStatus.Text = $"Đã đọc trang {currentPageIndex}/ bản ghi thứ {itemIndex}";
                    Application.DoEvents();

                    var item = new VanBanDuThaoItem();

                    var titleLinkNode = itemNode.SelectSingleNode(".//a[contains(@class, 'text-bold')]");
                    if (titleLinkNode != null)
                    {
                        item.TenDuThao = CleanText(titleLinkNode.InnerText);
                        item.DetailUrl = ToAbsoluteUrl(pageUrl, titleLinkNode.GetAttributeValue("href", ""));
                    }

                    var divs = itemNode.SelectNodes(".//div");
                    if (divs != null)
                    {
                        for (int k = 0; k < divs.Count; k++)
                        {
                            var text = CleanText(divs[k].InnerText);
                            if (text.Contains("Ngày bắt đầu:"))
                            {
                                if (k + 1 < divs.Count) item.NgayBatDau = ParseDate(CleanText(divs[k + 1].InnerText));
                            }
                            else if (text.Contains("Ngày hết hạn:"))
                            {
                                if (k + 1 < divs.Count) item.NgayKetThuc = ParseDate(CleanText(divs[k + 1].InnerText));
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(item.DetailUrl))
                    {
                        await ParseDetailAsync(httpClient, item, item.DetailUrl);
                    }

                    result.Add(item);
                }"""
code = code.replace(read_old, read_new)

# Replace ParseDetailAsync
detail_old = """                var tableNode = doc.DocumentNode.SelectSingleNode("//div[contains(@id, 'tabs-description')]//div[@class='table-responsive']//table");
                if (tableNode != null)
                {
                    var tbody = tableNode.SelectSingleNode(".//tbody");
                    if (tbody != null)
                    {
                        var coQuanBanHanhNode = tbody.SelectSingleNode(".//tr[3]//td[1]//div[1]");
                        if (coQuanBanHanhNode != null) item.CoQuanBanHanh = CleanText(coQuanBanHanhNode.InnerText);

                        var nguoiKyNode = tbody.SelectSingleNode(".//tr[3]//td[1]//div[2]");
                        if (nguoiKyNode != null) item.NguoiKy = CleanText(nguoiKyNode.InnerText);

                        var ngayCoHieuLucNode = tbody.SelectSingleNode(".//tr[3]//td[2]");
                        if (ngayCoHieuLucNode != null) item.NgayCoHieuLuc = ParseDate(CleanText(ngayCoHieuLucNode.InnerText));

                        var ngayHetHieuLucNode = tbody.SelectSingleNode(".//tr[4]//td[2]");
                        if (ngayHetHieuLucNode != null) item.NgayHetHieuLuc = ParseDate(CleanText(ngayHetHieuLucNode.InnerText));
                    }
                }

                var attachTableNode = doc.DocumentNode.SelectSingleNode("//div[@id='tabs-description-3-11']//table");
                if (attachTableNode != null)
                {
                    var aNodes = attachTableNode.SelectNodes(".//tr//td//a");"""

detail_new = """                var loaiVbNode = doc.DocumentNode.SelectSingleNode("//th[contains(text(), 'Loại văn bản')]/following-sibling::td");
                if (loaiVbNode != null) item.LoaiVanBan = CleanText(loaiVbNode.InnerText);

                var linhVucNode = doc.DocumentNode.SelectSingleNode("//th[contains(text(), 'Lĩnh vực văn bản')]/following-sibling::td");
                if (linhVucNode != null) item.LinhVuc = CleanText(linhVucNode.InnerText);

                var noiDungNode = doc.DocumentNode.SelectSingleNode("//div[@class='brief-vb']");
                if (noiDungNode != null) item.NoiDungDuThao = noiDungNode.InnerHtml.Trim();

                var attachTableNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'box-file-download')]//table | //table[.//a[contains(@href, '/upload/')]] | //table");
                if (attachTableNode != null)
                {
                    var aNodes = attachTableNode.SelectNodes(".//tr//td//a[contains(@href, '/upload/')]");"""
code = code.replace(detail_old, detail_new)


# Replace btnSaveData_Click
save_old = """                            if (string.IsNullOrWhiteSpace(item.SoKyHieu))
                            {
                                item.SoKyHieu = item.LoaiVanBan;
                            }
                            if (string.IsNullOrWhiteSpace(item.SoKyHieu))
                            {
                                continue;
                            }
                                
                                
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var existingLaw = await dbVanBan.Laws.FirstOrDefaultAsync(l => l.DetailLinkClone == item.DetailUrl);
                            if (existingLaw != null)
                            {
                                duplicateList.Add($"- Số KH: {item.SoKyHieu}\\n  Link: {item.DetailUrl}");
                                continue;
                            }

                            // Xử lý Category / Status
                            string idEffectStatus  = string.Empty;
                            if (!string.IsNullOrEmpty(item.TinhTrangHieuLuc))
                            {
                                var effect = dbVanBan.EffectStatus.FirstOrDefault(x => x.Title == item.TinhTrangHieuLuc);
                                if (effect == null)
                                {
                                    effect = new EffectStatu { Id = QHCommons.GenAutoId(), Title = item.TinhTrangHieuLuc,
                                        Language = "vi", IsShow = true
                                    };
                                    dbVanBan.EffectStatus.Add(effect);
                                    dbVanBan.SaveChanges();
                                }
                                idEffectStatus = effect.Id;
                            }

                            string idTypeOfDocument = string.Empty;
                            if (string.IsNullOrEmpty(item.LoaiVanBan))
                            {
                                item.LoaiVanBan = "Khác";
                            }
                            var type = dbVanBan.TypeOfDocuments.FirstOrDefault(x => x.Title == item.LoaiVanBan);
                            if (type == null)
                            {
                                var typeID = QHCommons.GenAutoId();
                                type = new TypeOfDocument { Id = typeID, Title = item.LoaiVanBan, Language = "vi", CreatedBy = "admin", IsShow = true };
                                dbVanBan.TypeOfDocuments.Add(type);
                                dbVanBan.SaveChanges();
                                idTypeOfDocument = typeID;
                            }
                            else
                            {
                                idTypeOfDocument = type.Id;
                            }

                            if (string.IsNullOrWhiteSpace(idTypeOfDocument)) continue;

                            // Tạo Law
                            var law = new Law
                            {
                                Id = QHCommons.GenAutoId(),
                                Title = item.SoKyHieu,
                                Status = 1,
                                OfficialNumber = item.SoKyHieu,
                                PublishedDate = item.NgayBanHanh ?? DateTime.Now,
                                EffectiveDate = item.NgayCoHieuLuc,
                                ExpiryDate = item.NgayHetHieuLuc,
                                PublicDate = item.NgayBanHanh ?? DateTime.Now,
                                Source = item.TrichYeu,
                                EffectiveArea = string.Empty,
                                Content = string.Empty,
                                CreatedBy = "admin",
                                IdEffectStatus = idEffectStatus,
                                IdTypeOfDocument = idTypeOfDocument,
                                VanBanQCTC = vanBanQCTC,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                ExtraProperties = "{}",
                                Language = "vi",
                                CreationTime = DateTime.Now,
                                DetailLinkClone = item.DetailUrl,
                                TypeOfDocumentTitle = item.LoaiVanBan,
                                IsDeleted = false
                            };
                            dbVanBan.Laws.Add(law);
                            dbVanBan.SaveChanges();

                            // Tạo LawSigner
                            string idSigner = string.Empty;

                            item.NguoiKy = string.IsNullOrWhiteSpace(item.NguoiKy) ? item.CoQuanBanHanh : item.NguoiKy;

                            if (!string.IsNullOrEmpty(item.NguoiKy))
                            {
                                var signer = dbVanBan.Signers.FirstOrDefault(x => x.Title == item.NguoiKy);
                                if (signer == null)
                                {
                                    signer = new Signer { Id = QHCommons.GenAutoId(), Title = item.NguoiKy, CreatedBy = "admin" , IsShow = true};
                                    dbVanBan.Signers.Add(signer);
                                    dbVanBan.SaveChanges();
                                }
                                idSigner = signer.Id;
                            }

                            string idPromulgator = string.Empty;
                            if (!string.IsNullOrEmpty(item.CoQuanBanHanh))
                            {
                                var promulgator = dbVanBan.Promulgators.FirstOrDefault(x => x.Title == item.CoQuanBanHanh);
                                if (promulgator == null)
                                {
                                    promulgator = new Promulgator { Id = QHCommons.GenAutoId(), Title = item.CoQuanBanHanh, Language = "vi", CreatedBy = "admin" , IsShow = true };
                                    dbVanBan.Promulgators.Add(promulgator);
                                    dbVanBan.SaveChanges();
                                }
                                idPromulgator = promulgator.Id;
                            }

                            if (!string.IsNullOrWhiteSpace(idSigner) 
                                && !string.IsNullOrWhiteSpace(idPromulgator))
                            {
                                var lawSigner = new LawSigner
                                {
                                    Id = Guid.NewGuid(),
                                    IdLaw = law.Id,
                                    IdSigner = idSigner,
                                    IdPromulgator = idPromulgator,
                                    Position = ""
                                };
                                dbVanBan.LawSigners.Add(lawSigner);
                                dbVanBan.SaveChanges();
                            }

                            // Tạo LawField
                            string idField = null;
                            if (!string.IsNullOrEmpty(item.LinhVuc))
                            {
                                var field = dbVanBan.Fields.FirstOrDefault(x => x.Title == item.LinhVuc);
                                if (field == null)
                                {
                                    field = new Field { Id = QHCommons.GenAutoId(), Title = item.LinhVuc, Language = "vi", CreatedBy = "admin", IsShow = true };
                                    dbVanBan.Fields.Add(field);
                                    dbVanBan.SaveChanges();
                                }
                                idField = field.Id;
                            }

                            if (!string.IsNullOrWhiteSpace(idField))
                            {
                                var lawField = new LawField
                                {
                                    Id = Guid.NewGuid(),
                                    IdLaw = law.Id,
                                    IdField = idField
                                };
                                dbVanBan.LawFields.Add(lawField);
                                dbVanBan.SaveChanges();
                            }

                            // Tải file đính kèm
                            foreach (var attach in item.Attachments)
                            {
                                string fileNameToDownload = string.IsNullOrWhiteSpace(attach.FileName) ? GetFileNameFromUrl(attach.Url) : attach.FileName;
                                string localFilePath = await DownloadFileAsync(httpClient, attach.Url, downloadFolder, fileNameToDownload);

                                if (!string.IsNullOrEmpty(localFilePath))
                                {
                                    var file = new CMSFile()
                                    {
                                        Id = Guid.NewGuid(),
                                        FileType = 2,
                                        CreationTime = DateTime.Now,
                                        FileContainerName = "CMSContainerPublic",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        ExtraProperties = "{}",
                                        MimeType = GetMimeType(attach.FileName),
                                        FileExtention = 2,
                                        Language = "vi",
                                        FullPathServer = localFilePath.Replace("C:\\", "/").Replace("\\", "/"),
                                        FileName = attach.FileName
                                    };
                                    dbFiles.CMSFiles.Add(file);
                                    dbFiles.SaveChanges();
                                    
                                    var fileAttach = new CMSFileAttachment()
                                    {
                                        Id = Guid.NewGuid(),
                                        EntityId = law.Id,
                                        FileId = file.Id,
                                        ExtraProperties = "{}",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        CreationTime = DateTime.Now,
                                        FileAttachmentType = 2
                                    };
                                    dbFiles.CMSFileAttachments.Add(fileAttach);
                                    dbFiles.SaveChanges();
                                }
                            }"""

save_new = """                            if (string.IsNullOrWhiteSpace(item.TenDuThao))
                            {
                                continue;
                            }
                                
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var existingDuThao = await dbVanBan.VanBanDuThaos.FirstOrDefaultAsync(l => l.DetailLinkClone == item.DetailUrl);
                            if (existingDuThao != null)
                            {
                                duplicateList.Add($"- Tên dự thảo: {item.TenDuThao}\\n  Link: {item.DetailUrl}");
                                continue;
                            }

                            string idTypeOfDocument = string.Empty;
                            if (string.IsNullOrEmpty(item.LoaiVanBan))
                            {
                                item.LoaiVanBan = "Khác";
                            }
                            var type = dbVanBan.TypeOfDocuments.FirstOrDefault(x => x.Title == item.LoaiVanBan);
                            if (type == null)
                            {
                                var typeID = QHCommons.GenAutoId();
                                type = new TypeOfDocument { Id = typeID, Title = item.LoaiVanBan, Language = "vi", CreatedBy = "admin", IsShow = true };
                                dbVanBan.TypeOfDocuments.Add(type);
                                dbVanBan.SaveChanges();
                                idTypeOfDocument = typeID;
                            }
                            else
                            {
                                idTypeOfDocument = type.Id;
                            }

                            // Tạo VanBanDuThao
                            var duThao = new VanBanDuThao
                            {
                                Id = QHCommons.GenAutoId(),
                                TrichYeu = item.TenDuThao,
                                NoiDung = item.NoiDungDuThao,
                                NgayBatDau = item.NgayBatDau,
                                NgayKetThuc = item.NgayKetThuc,
                                Status = 1,
                                CreatedBy = "admin",
                                IdTypeOfDocument = string.IsNullOrWhiteSpace(idTypeOfDocument) ? null : idTypeOfDocument,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                ExtraProperties = "{}",
                                Language = "vi",
                                CreationTime = DateTime.Now,
                                DetailLinkClone = item.DetailUrl,
                                IsDeleted = false
                            };
                            dbVanBan.VanBanDuThaos.Add(duThao);
                            dbVanBan.SaveChanges();

                            // Tạo VBDTField
                            string idField = null;
                            if (!string.IsNullOrEmpty(item.LinhVuc))
                            {
                                var field = dbVanBan.Fields.FirstOrDefault(x => x.Title == item.LinhVuc);
                                if (field == null)
                                {
                                    field = new Field { Id = QHCommons.GenAutoId(), Title = item.LinhVuc, Language = "vi", CreatedBy = "admin", IsShow = true };
                                    dbVanBan.Fields.Add(field);
                                    dbVanBan.SaveChanges();
                                }
                                idField = field.Id;
                            }

                            if (!string.IsNullOrWhiteSpace(idField))
                            {
                                var vbdtField = new VBDTField
                                {
                                    Id = Guid.NewGuid(),
                                    IdVanBanDuThao = duThao.Id,
                                    IdField = idField
                                };
                                dbVanBan.VBDTFields.Add(vbdtField);
                                dbVanBan.SaveChanges();
                            }

                            // Tải file đính kèm
                            foreach (var attach in item.Attachments)
                            {
                                string fileNameToDownload = string.IsNullOrWhiteSpace(attach.FileName) ? GetFileNameFromUrl(attach.Url) : attach.FileName;
                                string localFilePath = await DownloadFileAsync(httpClient, attach.Url, downloadFolder, fileNameToDownload);

                                if (!string.IsNullOrEmpty(localFilePath))
                                {
                                    var file = new CMSFile()
                                    {
                                        Id = Guid.NewGuid(),
                                        FileType = 2,
                                        CreationTime = DateTime.Now,
                                        FileContainerName = "CMSContainerPublic",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        ExtraProperties = "{}",
                                        MimeType = GetMimeType(attach.FileName),
                                        FileExtention = 2,
                                        Language = "vi",
                                        FullPathServer = localFilePath.Replace("C:\\\\", "/").Replace("\\\\", "/"),
                                        FileName = attach.FileName
                                    };
                                    dbFiles.CMSFiles.Add(file);
                                    dbFiles.SaveChanges();
                                    
                                    var fileAttach = new CMSFileAttachment()
                                    {
                                        Id = Guid.NewGuid(),
                                        EntityId = duThao.Id,
                                        FileId = file.Id,
                                        ExtraProperties = "{}",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        CreationTime = DateTime.Now,
                                        FileAttachmentType = 2
                                    };
                                    dbFiles.CMSFileAttachments.Add(fileAttach);
                                    dbFiles.SaveChanges();
                                }
                            }"""
code = code.replace(save_old, save_new)

code = code.replace("- Số KH:", "- Tên dự thảo:")

with open('FormCloneMoetVanBanDuThao.cs', 'w', encoding='utf-8') as f:
    f.write(code)

