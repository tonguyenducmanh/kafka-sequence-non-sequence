# Project test việc chạy worker lắng nghe queue kafka sequence ( tuần tự) và non sequence ( không tuần tự ) trong cùng 1 pod worker

sẽ có 1 số trường hợp user mong muốn các queue của mình được sequence vào trong 1 worker cụ thể để xử lý, khi đó chỉ cần sử dụng routing key là được

vd hình dưới là 1 topic kafka có 100 partition ( phải tạo bằng tay thì mới được 100, nếu start project để tự tạo topic thì nó sẽ chỉ có 1 partitions)

![alt text](<imgs/2 partition chia đều cho 2 worker.png>)

các parition này sẽ được chia đều cho 2 worker đang handle

![worker replica](<imgs/routing key theo worker.png>)

tuy nhiên 1 vài trường hợp khi triển khai mong muốn 1 worker sẽ có nhiều thread xử lý, khi đó khả năng nhiều thread cùng insert, update dữ liệu sẽ dẫn tới không đảm bảo đúng thứ tự sequence

test theo các bước dưới đây để thấy được sự khác biệt

bước 1: cài docker desktop

bước 2: chạy command docker-compose up trong thư mục [thư mục kafka-docker](kafka-docker)

```
docker-compose -f kafka-docker/docker-compose.yml down && docker-compose -f kafka-docker/docker-compose.yml up -d
```

bước 3: Mở project bằng visual studio, chạy debug cả KafkaConsumerWorker và KafkaPublish.API

với api sampe-pubish, 2 lời bài hát sau được publish liên lục và kết quả console log ở bên dưới

mỗi task được hiểu là 1 thread trong pc xử lý

(sửa config chạy tuần tự ở trong appsettings.json cờ UsingSequence)

kết quả chạy không tuần tự

![alt text](<imgs/chạy không tuần tự.png>)

kết quả chạy tuần tự

![alt text](<imgs/chạy tuần tự.png>)
