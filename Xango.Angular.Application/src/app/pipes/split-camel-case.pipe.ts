import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'splitCamelCase'
})
export class SplitCamelCasePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) return '';

    // Insert a space before each capital letter except the first
    return value.replace(/([A-Z])/g, ' $1').trim();
  }
}
