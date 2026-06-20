import { Component, HostListener, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { CanComponentDeactivate } from '../guards/UnsavedChangesGuard';
import Swal from 'sweetalert2';
import {
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';

@Injectable()
export class CanComponentDeactivatable implements CanComponentDeactivate {
  public form: FormGroup | any = null;

  constructor (
  ) {
  }
  canDeactivate(): Promise<boolean> | boolean {
	// If form is not dirty — allow navigation
	if (!this.form || !this.form.dirty) {
	  return true;
	}
	return Swal.fire({
	  title: 'You have unsaved changes',
	  text: 'Do you really want to leave without saving?',
	  icon: 'warning',
	  showCancelButton: true,
	  confirmButtonText: 'Leave',
	  cancelButtonText: 'Stay',
	  customClass: {
		popup: 'rounded-swal'
	  }
	}).then(result => !!result.isConfirmed);
  }

  @HostListener('window:beforeunload', ['$event'])
  beforeUnloadHandler(event: BeforeUnloadEvent) {
	if (this.form && this.form.dirty) {
	  event.preventDefault();
	  event.returnValue = '';
	}
  }

}

