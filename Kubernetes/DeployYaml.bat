@echo off
kubectl apply -f mssql.yaml
kubectl apply -f auth-api.yaml
kubectl apply -f coupon-api.yaml
kubectl apply -f inventory-api.yaml
kubectl apply -f product-api.yaml
kubectl apply -f order-api.yaml
kubectl apply -f shoppingcart-api.yaml
kubectl apply -f ui.yaml
kubectl apply -f ingress.yaml
echo Done...
pause