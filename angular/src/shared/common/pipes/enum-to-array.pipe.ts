import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumToArray',
})
export class EnumToArrayPipe implements PipeTransform {
  transform(value): any {
    let keys = [];
    for (var enumMember in value) {
      if (!isNaN(parseInt(enumMember, 10))) {
        keys.push({ key: enumMember, value: value[enumMember] });
        // Uncomment if you want log
        // console.log("enum member: ", value[enumMember]);
      }
    }
    return keys;
  }
}
