import { PhotoModel } from './photo.model';

export class UserModel {
  public constructor(
    public id: number,
    public username: string,
    public knownAs: string,
    public age: number,
    public gender: string,
    public created: Date,
    public lastActive: Date,
    public photoUrl: string,
    public city: string,
    public country: string,
    public interests?: string,
    public introduction?: string,
    public lookingFor?: string,
    public photos?: PhotoModel[],
    public roles?: string[]
  ) {}
}
