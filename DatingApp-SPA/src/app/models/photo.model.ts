export class PhotoModel {
  public constructor(
    public id: number,
    public url: string,
    public description: string,
    public dateAdded: Date,
    public isMain: boolean,
    public isApproved: boolean
  ) {}
}
