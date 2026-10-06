1. Ban tạo cho tôi 1 form clone dữ liệu từ 1 trang danh sách văn bản.

- Thông tin file ảnh hoặc file thì lưu giúp tôi vào ổ c:\uploadFckFiles\vanban

2. Bạn hãy sử dụng 2 Model trong project `QHBASE`:

- `Moet_VanBans.Context.cs` các thông tin entity sử dụng:
  `           public virtual DbSet<EffectStatu> EffectStatus { get; set; }
public virtual DbSet<Field> Fields { get; set; }
public virtual DbSet<GopYVanBanDuThao> GopYVanBanDuThaos { get; set; }
public virtual DbSet<Law> Laws { get; set; }
public virtual DbSet<LawCategory> LawCategories { get; set; }
public virtual DbSet<LawField> LawFields { get; set; }
public virtual DbSet<LawSigner> LawSigners { get; set; }
public virtual DbSet<Promulgator> Promulgators { get; set; }
public virtual DbSet<Signer> Signers { get; set; }
public virtual DbSet<TuLieuAnPham> TuLieuAnPhams { get; set; }
public virtual DbSet<TypeOfDocument> TypeOfDocuments { get; set; }
public virtual DbSet<VanBanDuThao> VanBanDuThaos { get; set; }`
- `Moet_Files.Context.cs` lưu thông tin file cho Văn bản

3. Thông tin file đọc html

- Danh sách: file view của bài viết tham khảo: `@plans\Moets\VanBan\List.html` bạn cần bóc từ thẻ `list-legal-document-table\items` lấy ra các thẻ item văn bản:
  - `Trích yếu`: đọc trong thẻ `div[@class='width-col-12 width-col-md-6 border-right border-dotted border-width-2xs border-color-gray']`
  - `Loại văn bản`: đọc trong thẻ `div[@class='properties']\table\tbody\tr[2]\td[2]`
  - `Lĩnh vực`: đọc trong thẻ `div[@class='properties']\table\tbody\tr[3]\td[2]`
  - `Số ký hiệu`: đọc trong thẻ `div[@class='properties']\table\tbody\tr[1]\td[2]`
  - `Ngày ban hành`: đọc trong thẻ `div[@class='properties']\table\tbody\tr[1]\td[4]`
  - `Tình trạng hiệu lực`: đọc trong thẻ `div[@class='properties']\table\tbody\tr[4]\td[2]`
  - `Link bài viết`: đọc trong thẻ `div[@class='title']\a`
- Chi tiết Bài viết: file view của bài viết tham khảo: `@plans\Moets\VanBan\Detail.html` các thẻ cần bóc tách: `div[id='tabs-description-2-11']\div[@class='table-responsive']\table`, thẻ table có dạng: `<table class="table table-bordered table-striped">
  <tbody>
  <tr>
  <th scope="col" style="width: 25%">Số ký hiệu</th>
  <td style="width: 25%">46/2026/TT-BGDĐT</td>
  <th scope="col" style="width: 25%">Ngày ban hành</th>
  <td style="width: 25%">-</td>
  </tr>
  <tr>
  <th scope="col">Loại văn bản</th>
  <td>Thông tư</td>
  <th scope="col">Lĩnh vực</th>
  <td>Giáo dục nghề nghiệp, Giáo dục thường xuyên</td>
  </tr>
  <tr>
  <th scope="col">Cơ quan ban hành/ Người ký</th>
  <td>
  <div>Bộ Giáo dục và Đào tạo</div>
  <div>Thứ trưởng Lê Quân</div>
  </td>
  <th scope="col">Ngày có hiệu lực</th>
  <td>-</td>
  </tr>
  <tr>
  <th scope="col">Tình trạng hiệu lực</th>
  <td></td>
  <th scope="col">Ngày hết hiệu lực</th>
  <td>-</td>
  </tr>
  </tbody>
    </table>`
  Hãy bóc tách các thẻ trong thẻ `tr` cho hợp lý:
  - `Cơ quan ban hành`: `table\tbody\tr[3]\td[2]\div[1]`
  - `Người ký`: `table\tbody\tr[3]\td[2]\div[2]`
  - `Ngày có hiệu lực`: `table\tbody\tr[3]\td[4]`
  - `Ngày hết hiệu lực`: `table\tbody\tr[4]\td[4]`
  Bóc tách file đính kèm bạn hãy dựa vào thẻ `div[id='tabs-description-3-11']\table` đọc các thẻ `tr` và bóc tách các thẻ `td` cho hợp lý: `<td><a rel="noopener" href="/upload/2007219/20260610/Die____u_le_____tru__o____ng_THN_06da8.pdf" download="" target="_blank"></a></td>` => `<td><a rel="noopener" href="/upload/2007219/20260610/Die____u_le_____tru__o____ng_THN_06da8.pdf" download="" target="_blank">46-2026-TT-BGDĐT.pdf</a></td>` => Bóc tách `Tên file` và `LinkFile`.

Từ 2 dữ liệu bóc tách trên bạn hãy tạo 1 class để thể hiện dữ liệu đã bóc tách ra.

3. Form đó có các ô cần nhập như sau:
   - url trang web
   - Từ trang
   - Đến trang (vd: https://moet.gov.vn/van-ban/van-ban-chi-dao-dieu-hanh?&orderBy=issueTime%20DESC&itemsPerPage=24&pageNo=106) thay đổi pageNo cho hợp lý khi người dùng nhập Từ trang và Đến trang
   - Folder lưu trữ
   - Combobox chọn loại văn bản: Văn bản quy phạm pháp luật (giá trị 1) và Văn bản chỉ đạo, điều hành (giá trị 2)
4. Khi điền đầy đủ thông tin thì có 2 nút
   - Đọc dữ liệu => Bắt đầu đọc dữ liệu và hiển thị ra view để xem danh sách bài viết đã lấy được
   - Lưu dữ liệu:
     - Lưu => Đã lưu:
       Bước 1: Lưu `Law`
       - Title: `Loại văn bản` + `Số ký hiệu` => "Thông tư 46/2026/TT-BGDĐT"
       - Status: 1
       - OfficialNumber: `Số ký hiệu`
       - PublishedDate: `Ngày ban hành`
       - EffectiveDate: `Ngày có hiệu lực`
       - ExpiryDate: `Ngày hết hiệu lực`
       - PublicDate: `Ngày công bố`
       - Source: `Cơ quan ban hành`
       - EffectiveArea: `Lĩnh vực`
       - Content: `Nội dung`
       - CreatedDate: `Ngày tạo`
       - CreatedBy: `Người tạo`
       - CreatorId: `Người tạo`
       - IdEffectStatus: Id trạng thái hiệu lực => Dựa vào`Tình trạng hiệu lực` để filter Id trong table `EffectStatus` (nếu chưa có thì tạo mới và gán lại Id mới)
       - IdTypeOfDocument: Id loại văn bản => Dựa vào `Loại văn bản` để filter Id trong table `TypeOfDocument` (nếu chưa có thì tạo mới và gán lại Id mới)
       - VanBanQCTC: `Combobox chọn loại văn bản`
         Bước 2: Lưu `LawSigner`:
         - `IdLaw` : Id của Law.Id
         - `IdSigner`: Id người ký => Dựa vào`Người ký` để filter Id trong table `Signer` (nếu chưa có thì tạo mới và gán lại Id mới)
         - `IdPromulgator`: Id Cơ quan ban hành => Dựa vào `Cơ quan ban hành` để filter Id trong table `Promulgator` (nếu chưa có thì tạo mới và gán lại Id mới)
         - `Position`: để trống
         - `Id`: sinh mã guid tự động
           Bước 3: Lưu `LawField`
         - `IdLaw` : Id của Law.Id
         - `IdField`: Id lĩnh vực => Dựa vào `Lĩnh vực` để filter Id trong table `Field` (nếu chưa có thì tạo mới và gán lại Id mới)
         - - `Id`: sinh mã guid tự động
             Bước 4: Lưu `Moet_Files`
         - `IdLaw` : Id của Law.Id
         - `Id` : sinh mã guid tự động
