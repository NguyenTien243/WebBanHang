Để có thể chạy được ứng dụng, máy tính phải có các yêu cầu sau:

1. Yêu cầu và cài đặt FE: 
	Cài đặt npm.
	Cài đặt NodeJS.
	Cài đặt AngularJS.
	Cần có Windows Terminal hoặc Command Prompt.
	Các bước để tiến hành cài đặt:

	*Bước 1: Clone hoặc download project từ link github (Link github: https://github.com/ngoclam9200/tieuluanchuyennganh)

	*Bước 2: Khởi chạy Front-end.

	Sau khi clone project về máy, chuột phải vào thư mục front và chọn mở bằng Visual Studio Code.

	Mở terminal mới. Sau khi Windows Terminal hiện ra, chạy lệnh "npm install" để cập nhật các thư viện và package cần thiết để project hoạt động.

	Tiếp tục chạy lệnh "npm start" trên Windows Terminal để bắt đầu chạy Front-end.
 
	*Bước 3: Cấu hình Web API cho FE

	Mặc định sử dụng Web API đã deploy là: "https://webbanhangapitienlam.azurewebsites.net", thay đổi Web API theo mong muốn.

	Có thể thay đổi host API phía Back-end theo đường dẫn thư mục: /src/services/api.servicve.ts

2. Yêu cầu và cài đặt BE:
	Cài đặt SQL Server.
	Cài đặt Visual Studio (khuyến nghị bản Visual Studio 2019 Community).
	Cần có ASP.NET Runtime 5.0
	Cần có Windows Terminal hoặc Command Prompt.
	Các bước để tiến hành cài đặt:

	*Bước 1: Clone hoặc download project từ link github (Link github: https://github.com/NguyenTien243/WebBanHang)

	*Bước 2: Khởi chạy Back-end.

	Sau khi clone project về máy, mở project bằng Visual Studio.

	Có thể chạy Back-end bằng "IIS Express" hoặc "WebBanHangAPI".

	*Lưu ý: Khi chạy bằng "IIS Express" sẽ chạy bằng localhost cổng 44371 (https://localhost:44371), 
		còn với "WebBanHangAPI" sẽ chạy bằng localhost cổng 5001 (https://localhost:5001) 
 
	*Bước 3: File cấu hình cho BE

	Có thể điều chỉnh các thông tin như chuỗi kết nối Database, Thiết lập Email, thiết lập thanh toán Paypal trong file "appsettings.json".

