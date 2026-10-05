1. Ban tạo cho tôi 1 form clone dữ liệu từ 1 danh sách Video. (html mẫu ở file `html.html`)

- Người dùng nhập các HTML để bóc tách như file mẫu.
- Thông tin file ảnh hoặc file thì lưu giúp tôi vào ổ c:\uploadFckFiles\tinvideo

2. Bạn hãy sử dụng 2 Model trong project `QHBASE`:

- `Moet_HinhAnh.edmx` các thông tin entity sử dụng
  - `CategoryVideo`: Danh sách danh mục Video:
    `public partial class CategoryVideo
    {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public CategoryVideo()
    {
    this.Videos = new HashSet<Video>();
    }

        public System.Guid Id { get; set; }
        public Nullable<System.Guid> TenantId { get; set; }
        public string Title { get; set; }
        public Nullable<int> Order { get; set; }
        public bool IsShow { get; set; }
        public bool Shared { get; set; }
        public Nullable<System.Guid> ParentID { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string PageUrl { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public long CategoryIndex { get; set; }
        public string TreeParentIds { get; set; }
        public string DetailCategoryRoot { get; set; }
        public string Language { get; set; }
        public string OrganizationUnitCode { get; set; }
        public Nullable<System.Guid> OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public string ExtraProperties { get; set; }
        public string ConcurrencyStamp { get; set; }
        public System.DateTime CreationTime { get; set; }
        public Nullable<System.Guid> CreatorId { get; set; }
        public Nullable<System.DateTime> LastModificationTime { get; set; }
        public Nullable<System.Guid> LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.Guid> DeleterId { get; set; }
        public Nullable<System.DateTime> DeletionTime { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Video> Videos { get; set; }

    }`

  - `Video`: Danh sách Video:
    ` public partial class Video
    {
    public System.Guid Id { get; set; }
    public Nullable<System.Guid> TenantId { get; set; }
    public string Title { get; set; }
    public Nullable<int> Order { get; set; }
    public Nullable<int> View { get; set; }
    public Nullable<bool> Shared { get; set; }
    public Nullable<bool> Hot { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public string File { get; set; }
    public string Image { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public string DeletedBy { get; set; }
    public System.Guid CategoryVideoId { get; set; }
    public string CategoryVideoTree { get; set; }
    public string CategoryVideoTitle { get; set; }
    public string OrganizationUnitCode { get; set; }
    public Nullable<System.Guid> OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; }
    public long VideoIndex { get; set; }
    public string Language { get; set; }
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
    public Nullable<System.DateTime> NgayTao { get; set; }
    public string Author { get; set; }

            public virtual CategoryVideo CategoryVideo { get; set; }

        }`

3. Thông tin file đọc html

- từ file `html.html` (danh sách video) hãy bóc thông tin item thẻ `article-item`:
  - Thẻ a lấy ra Link detail bài viết
  - Thẻ img lấy ra đường dẫn ảnh đại diện
  - div[class='article-title common-title'] lấy Tiêu đề
  - Từ Link detail bài viết bạn cần lấy ra các thông tin trong file `detailVideo.html` thẻ div[@class='detail-video']: - Link Video (đọc src trong thẻ video) - Mô tả (đọc nội dung trong thẻ div[@class='article-brief']) - Ngày tạo (đọc nội dung trong thẻ div[@class='article-time margin-bottom-lg']) - Nguồn (đọc nội dung trong thẻ div[@class='author'])
    sau khi bạn đọc thì hãy tạo ra 1 class để map dữ liệu bóc tách ra: `VideoCloneItem`:
    - `Title` : Gán Tiêu đề bài viết
    - `Description` : Mô tả
    - `Author` : Nguồn
    - `ViewCount` : ViewCount (đọc nội dung trong thẻ div[@class='author'])
    - `SourceUrl` : Link detail bài viết
    - `VideoUrl` : Link Video
    - `Image` : Ảnh đại diện Video
    - `LinkDetail`: Link detail bài viết
    - `NgayTao` : Ngày tạo (đọc nội dung trong thẻ div[@class='article-time margin-bottom-lg'])

3. Form đó có các ô cần nhập như sau:
   - Chọn danh mục video => Dropdownlist lấy từ `CategoryVideo`.
4. Khi điền đầy đủ thông tin thì có 2 nút
   - Đọc dữ liệu => Bắt đầu đọc dữ liệu và hiển thị ra view để xem danh sách video đã lấy được
   - Lưu dữ liệu:
     - Khi bạn foreach các item `VideoCloneItem`:
       - Hãy update các `VideoCloneItem.Image` và `VideoCloneItem.VideoUrl` vào thư mục chỉ định
     - Lưu `Video` => lưu các thông tin:
       - Title : `VideoCloneItem.Title`
       - Description : `VideoCloneItem.Description`
       - Author : `VideoCloneItem.Author`
       - View : `VideoCloneItem.ViewCount`
       - NgayTao : `VideoCloneItem.NgayTao`
       - SourceUrl : `VideoCloneItem.SourceUrl`
       - VideoUrl : `VideoCloneItem.VideoUrl`
       - Image : `VideoCloneItem.Image` trả về `Url`
       - DetailLinkClone: `VideoCloneItem.LinkDetail`
       - File: `VideoCloneItem.VideoUrl` trả về `Url`
       - CategoryVideoId: từ danh mục video chọn
