export class MessageModel {
  public constructor(
    public id: number,
    public senderId: number,
    public recipientId: number,
    public senderKnownAs: string,
    public recipientKnownAs: string,
    public senderPhotoUrl: string,
    public recipientPhotoUrl: string,
    public content: string,
    public isRead: boolean,
    public dateRead: Date,
    public dateSent: Date,
  ) {}
}
