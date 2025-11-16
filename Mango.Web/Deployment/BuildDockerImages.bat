cd ..
docker build -t auth-api:latest -f Xango.Services.AuthAPI\Dockerfile .
docker build -t coupon-api:latest -f Xango.Services.CouponAPI\Dockerfile .
docker build -t inventory-api:latest -f Xango.Services.InventoryAPI\Dockerfile .
docker build -t product-api:latest -f Xango.Services.ProductAPI\Dockerfile .
docker build -t order-api:latest -f Xango.Services.OrderAPI\Dockerfile .
docker build -t shoppingcart-api:latest -f Xango.Services.ShoppingCartAPI\Dockerfile .
docker build -t xangoweb:latest -f Mango.Web\Dockerfile .

echo Done...
pause