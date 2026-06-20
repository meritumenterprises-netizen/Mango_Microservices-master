export class ResponseDto {
  result : string;
  isSuccess: boolean;
  message : string;
  stackTrace : string;

  constructor()  {
    this.result = "";
    this.isSuccess = false;
    this.message = "";
    this.stackTrace = "";
  }
}

