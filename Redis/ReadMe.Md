### Run Docker with Id and Password 

docker run -d -p 6379:6379 --name redis redis redis-server --requirepass "123456"