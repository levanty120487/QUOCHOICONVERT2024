1. Ban tạo cho tôi 1 form clone dữ liệu từ 1 trang Danh sách thông báo.

- Trang nguồn: https://moet.gov.vn/tin-tuc-cot-trai/thong-tin-bao-chi-ve-giao-duc (chỉ có 1 trang danh sách duy nhất)
- Thông tin file ảnh hoặc file thì lưu giúp tôi vào ổ c:\uploadFckFiles\thongtinbaochi

2. Bạn hãy sử dụng 2 Model trong project `QHBASE`:

- `Moet_Commons.edmx` các thông tin entity sử dụng
  - `LoaiThongBao`: Danh sách danh mục thông báo:
    `public partial class LoaiThongBao
    {
    public string Id { get; set; }
    public string TieuDe { get; set; }
    public string MoTa { get; set; }
    public Nullable<int> TrangThai { get; set; }
    public string ExtraProperties { get; set; }
    public string ConcurrencyStamp { get; set; }
    public System.DateTime CreationTime { get; set; }
    public Nullable<System.Guid> CreatorId { get; set; }
    public Nullable<System.DateTime> LastModificationTime { get; set; }
    public Nullable<System.Guid> LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public Nullable<System.Guid> DeleterId { get; set; }
    public Nullable<System.DateTime> DeletionTime { get; set; }

        public virtual ICollection<DanhSachThongBao> DanhSachThongBaos { get; set; }

    }`

  - `DanhSachThongBao`: Danh sách thông báo:
    `public partial class DanhSachThongBao
    {
    public string Id { get; set; }
    public string TieuDe { get; set; }
    public string SoThongBao { get; set; }
    public string IdLoaiThongBao { get; set; }
    public string MoTa { get; set; }
    public string NoiDung { get; set; }
    public Nullable<System.DateTime> NgayThongBao { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string DeletedBy { get; set; }
    public Nullable<int> Status { get; set; }
    public string Language { get; set; }
    public string TieuDeUnicode { get; set; }
    public string MoTaUnicode { get; set; }
    public string Image { get; set; }
    public Nullable<int> ViewCount { get; set; }
    public string DetailLinkClone { get; set; }
    public string ExtraProperties { get; set; }
    public string ConcurrencyStamp { get; set; }
    public System.DateTime CreationTime { get; set; }
    public Nullable<System.Guid> CreatorId { get; set; }
    public Nullable<System.DateTime> LastModificationTime { get; set; }
    public Nullable<System.Guid> LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
    public Nullable<System.Guid> DeleterId { get; set; }
    public Nullable<System.DateTime> DeletionTime { get; set; }

        public virtual LoaiThongBao LoaiThongBao { get; set; }

    }`

3. Thông tin file đọc html

- Danh sách: file view của bài viết tham khảo: `@plans\Moets\DanhSachThongBao\List.html` bạn cần bóc thẻ `div[@class='table-responsive']/table[@class='table-cms-v2 table-striped']` bao gồm các cột:
  - STT: `td[@class='text-center']` lấy số thứ tự
  - Tên tài liệu: `td[@class='tg-yw4l']` lấy thẻ a và href, title
  - Ngày ban hành: `td[@class='text-center']` lấy nội dung
  - File đính kèm: `td[@class='text-center']` lấy thẻ a và href, title
- Chi tiết Bài viết: lấy link từ thẻ `a` trong: Tên tài liệu: `td[@class='tg-yw4l']` các thẻ cần bóc tách thêm:
  - Số lượt xem: div có thông tin: `<div class="news-date"><i class="vi vi-clock"></i>&nbsp;30/06/2026, 19:00:20 - <i class="vi vi-eye"></i> Lượt xem: 7</div>` lấy số lượt xem giúp tôi.

3. Form đó có các ô cần nhập như sau:
   - Chọn danh mục thông báo => Dropdownlist lấy từ `LoaiThongBao` có phân cấp theo cấp cha - con.
4. Khi điền đầy đủ thông tin thì có 2 nút
   - Đọc dữ liệu => Bắt đầu đọc dữ liệu và hiển thị ra view để xem danh sách bài viết đã lấy được
   - Lưu dữ liệu:
     - Lưu `DanhSachThongBao` => Đã lưu:
       - `TieuDe` : `Title`
       - `MoTa` : `Description`
       - `Author` : `Author`
       - `ViewCount` : `ViewCount`
       - `NgayThongBao` : `NgayThongBao` (Ngày ban hành)
     - Lưu `Moet_Files` => Hãy lấy thông tin file từ `DanhSachThongBao` thông qua `Id` với `FileType` = 8
