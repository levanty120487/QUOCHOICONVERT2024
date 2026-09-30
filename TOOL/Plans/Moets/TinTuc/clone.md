1. Ban tạo cho tôi 1 form clone dữ liệu từ 1 trang web tin tức.

- Thông tin file ảnh hoặc file thì lưu giúp tôi vào ổ c:\uploadFckFiles\news

2. Bạn hãy sử dụng 2 Model trong project `QHBASE`:

- `Moet_News.edmx` các thông tin entity sử dụng
  - `Category`: Danh sách danh mục tin tức
  - `New`: Danh sách bài viết
- `Moet_Files.edmx` lưu thông tin file cho bài viết

3. Thông tin file đọc html

- Danh sách: file view của bài viết tham khảo: `@plans\Moets\TinTuc\List.html` bạn cần bóc từ thẻ nav-item list-news-one => list-new: danh sách bài viết
- Chi tiết Bài viết: file view của bài viết tham khảo: `@plans\Moets\TinTuc\Detail.html` các thẻ cần bóc tách:
  - Mô tả: div có class = `article-brief`
  - Nội dung: div có class=`content-detail`
  - Tác giả: div có class=`author`
  - Số lượt xem: div có class=`vi vi-eye`
  - Tài liệu đính kèm: div có class=`ul-fileattach`

3. Form đó có các ô cần nhập như sau:
   - Chọn danh mục => Dropdownlist lấy từ `Category` có phân cấp theo cấp cha - con.
   - url trang web
   - Class chứa danh sách bài viết
   - Class chứa tiêu đề bài viết
   - Class chứa ngày đăng bài
   - Class chứa ảnh đại diện
   - Tổng số page
4. Khi điền đầy đủ thông tin thì có 2 nút
   - Đọc dữ liệu => Bắt đầu đọc dữ liệu và hiển thị ra view để xem danh sách bài viết đã lấy được
   - Lưu dữ liệu:
     - Lưu `New` => Đã lưu:
       - CategoryId => Lấy từ `Category`
       - Title
       - Description
       - Content
       - Author
       - ViewCount
       - ImageUrl
       - CreatedDate
       - CreatedBy
     - ## Lưu `Moet_Files` => Hãy lấy thông tin file từ `New` thông qua `NewId`
