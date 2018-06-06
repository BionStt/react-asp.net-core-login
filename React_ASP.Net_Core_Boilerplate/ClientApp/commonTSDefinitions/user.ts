export class User {
    userName: string;
    firstName: string;
    lastName: string;
    token: string;
    id: number;
    state: number;
    email:string;

    constructor() {
        this.userName='';
        this.firstName='';
        this.lastName='';
        this.token='';
        this.id = null;
        this.state=-1;
        this.email='';
    }
}