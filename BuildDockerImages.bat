docker build -t xangoservicesauthapi:latest -f Xango.Services.AuthAPI\Dockerfile .
docker build -t xangoservicescouponapi:latest -f Xango.Services.CouponAPI\Dockerfile .
docker build -t xangoservicesinventoryapi:latest -f Xango.Services.InventoryAPI\Dockerfile .
docker build -t xangoservicesproductapi:latest -f Xango.Services.ProductAPI\Dockerfile .
docker build -t xangoservicesorderapi:latest -f Xango.Services.OrderAPI\Dockerfile .
docker build -t xangoservicesshoppingcartapi:latest -f Xango.Services.ShoppingCartAPI\Dockerfile .
docker build -t xangoweb:latest -f Mango.Web\Dockerfile .

echo Done...
pause