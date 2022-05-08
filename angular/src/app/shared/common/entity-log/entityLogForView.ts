import { Moment } from '@node_modules/moment';

export class EntityLogForView {
  transaction: string;
  modificationTime: string;
  modifierUserName: string;
  modifierUserId: number;
  modifierTenantId: number;
  modifierTenantName: string;
  changesData: LogChangeItem[];
}

export interface LogChangeItem {
  propertyName: string;
  propertyValue: LogChangeItemValue;
}

export interface LogChangeItemValue {
  newValue: string;
  originalValue: string;
}
