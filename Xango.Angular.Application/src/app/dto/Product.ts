export interface Product {
	productId : number;
	name : string;
	price : number;
	description : string;
	categoryName : string;
	base64Image? : string | null;
	imageUrl? : string | null;
	imageLocalPath? : string | null;
	count : number;
	stockInventory : number;
}

