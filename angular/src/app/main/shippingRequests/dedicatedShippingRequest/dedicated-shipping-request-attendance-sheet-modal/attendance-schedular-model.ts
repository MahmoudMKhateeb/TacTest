export class AttendanceSchedularModel {
  id?: number;
  text: string;
  statusId: number;
  startDate: Date;
  endDate: Date;
  allDay?: boolean;

  constructor(obj: any) {
    this.id = obj.id;
    this.text = obj.text;
    this.startDate = obj.startDate;
    this.statusId = obj.statusId;
    this.endDate = obj.endDate;
    this.allDay = obj.allDay;
  }
}
