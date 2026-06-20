import { Component, HostListener, Injectable, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../dto/Product';
import { ProductService } from '../../services/ProductService';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ResponseDto } from '../../dto/ResponseDto';
import { ActivatedRoute, Router } from '@angular/router';
import { lessThanValidator, __VALIDATORS_TEST__ } from '../../validators/validators';
import { CanComponentDeactivate } from '../../guards/UnsavedChangesGuard';
import { RouterLink, RouterModule } from '@angular/router';
import { CanComponentDeactivatable } from '../../basecomponents/CanComponentDeactivatable';
import Swal from 'sweetalert2';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-product-edit-component',
  templateUrl: './edit-product-component.html',
  styleUrl: './edit-product-component.css',
  imports: [FormsModule, ReactiveFormsModule, CommonModule, RouterModule],
  providers: [ProductService],
})
export class ProductEditComponent extends CanComponentDeactivatable {
  productForm: FormGroup | any = null;
  product$: Observable<ResponseDto> | any = null;
  product: Product | any = null;
  response: ResponseDto | any = null;
  productId!: number;
  readonly maxFileSize = 5 * 1024 * 1024; // 5 MB
  readonly allowedTypes = ['image/png', 'image/jpeg', 'image/webp'];
  imagePreview: string | null = null;
  fileError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private toastr: ToastrService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    super();
    this.productId = parseInt(this.route.snapshot.paramMap.get('id') as string)!;
    this.productForm = this.fb.group(
      {
        productName: [
          '', 
          [
            Validators.required, 
            Validators.minLength(6), 
            Validators.maxLength(100)]],
        price: [
          '',
          [
            Validators.required,
            Validators.min(1.0),
            Validators.max(1000.0),
            Validators.pattern(/^\d*\.?\d+$/),
          ],
        ],
        description: [
          '', 
          [
            Validators.required, 
            Validators.minLength(4), 
            Validators.maxLength(2000)]],
        categoryName: 
        [
          '', 
          [
            Validators.required, 
            Validators.minLength(4), 
            Validators.maxLength(50)]],
        stockInventory: [
          '',
          [
            Validators.required,
            Validators.min(1),
            Validators.max(10000),
            Validators.pattern(/^\d+$/),
          ],
        ],
        fileInput: [
          null, []
        ],
        base64Image: [
          null, []
        ],
        fileName: [
          null, []
        ]
      },
      {
        //validators: lessThanValidator('discountAmount', 'minAmount'),
      },
    );

    
    this.form = this.productForm;
    
    this.product$ = this.productService.getProduct(this.productId);
    this.product$.subscribe({
      next: (responseDto: any) => {
        this.response = responseDto;
      },
      error: (err: string | undefined) => {
        console.error('Error', err);
      },
      complete: () => {
        console.log('Done');
        this.product = this.response.result;
        this.productForm.patchValue({
          productName: this.product.name,
          categoryName: this.product.categoryName,
          description: this.product.description,
          price: this.product.price,
          stockInventory: this.product.stockInventory,
          fileName: this.product.imageUrl
        });
        this.productForm.markAsPristine();
      },
    });
  }
  
  onFileSelected(files: FileList | null): void {
    if (!files || files.length === 0) return;
  
    const file: File = files[0];
  
    if (!this.allowedTypes.includes(file.type)) {
      this.fileError = 'Only PNG, JPEG, or WEBP images are allowed.';
      return;
    }
  
    if (file.size > this.maxFileSize) {
      this.fileError = 'File size must be smaller than 5 MB.';
      return;
    }
  
    this.fileError = null;
  
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result as string;
      const base64String = (reader.result as string).split(',')[1];
  
      // update reactive form
      this.productForm.patchValue({ base64Image: base64String });
      this.productForm.patchValue({ fileName: file.name });
    };
    reader.readAsDataURL(file);
  }
     
  
  get productName() {
    return this.productForm.get('productName');
  }

  get categoryName() {
    return this.productForm.get('categoryName');
  }

  get description() {
    return this.productForm.get('description');
  }

  get price() {
    return this.productForm.get('price');
  }

  get stockInventory() {
    return this.productForm.get('stockInventory');
  }

  get fileName() {
    return this.productForm.get('fileName');
  }

  get fileInput() {
    return this.productForm.get('fileInput');
  }

  get base64Image() {
    return this.productForm.get('base64Image');
  }

  
  onSubmit() {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    console.log(this.productForm.value.name);
    console.log(this.productForm.value.categoryName);
    console.log(this.productForm.value.description);
    console.log(this.productForm.value.price);
    console.log(this.productForm.value.stockInventory);
    console.log(this.productForm.value.fileName);
    console.log(this.productForm.value.base64Image);
    let updatedProduct: Product = {
      productId: this.productId,
      name: this.productName.value,
      categoryName: this.categoryName.value,
      description: this.description.value,
      price: this.price.value,
      stockInventory: this.stockInventory.value,
      imageUrl: null,
      count: 1
    };
    if (this.fileName.value != null) {
      updatedProduct.base64Image = this.base64Image.value ?? null;
      updatedProduct.imageUrl = this.fileName.value ?? null;
      updatedProduct.imageLocalPath = this.fileName.value ?? null;
    }
    this.product$ = this.productService.updateProduct(updatedProduct);
    this.product$.subscribe({
      error: (err: any) => {
        console.error('Error', err);
      },
      complete: () => {
          console.log('Done');
          this.productForm.markAsPristine();
          this.toastr.success('Product successfully updated');
          this.router.navigate(['/product']);
        }
    });

  }
}
