// loader.component.ts
import { Component } from '@angular/core';
import { LoaderService } from '../services/LoaderService';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loader',
  templateUrl: './loader-component.html',
  styleUrls: ['./loader-component.css'],
  imports: [CommonModule]
})
export class LoaderComponent {
  loading$: Observable<boolean>;
  constructor(private loader: LoaderService) {
    this.loading$ = this.loader.loading$;
  }
}
