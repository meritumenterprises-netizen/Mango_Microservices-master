// loader.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LoaderService {
  private _count = 0;
  private _loading$ = new BehaviorSubject<boolean>(false);
  public readonly loading$ = this._loading$.asObservable();

  show() {
    this._count++;
    if (this._count > 0) this._loading$.next(true);
  }

  hide() {
    this._count = Math.max(0, this._count - 1);
    if (this._count === 0) this._loading$.next(false);
  }

  reset() {
    this._count = 0;
    this._loading$.next(false);
  }
}
