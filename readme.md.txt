Start rabbitmq with this:
docker run --rm -it --hostname my-rabbit -p 15672:15672 -p 5672:5672 --cpus=2 rabbitmq/masstransit