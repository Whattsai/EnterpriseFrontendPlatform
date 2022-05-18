import { Companies } from "./Companies";
import { ER } from "./ER";
import { Memo } from "./Memo";
export class BonusInfo {
Selected : string = '';
Department : string = '';
Companies : Companies[] = new Array<Companies>();
ID : number = 0;
Name : string = '';
ER : ER[] = new Array<ER>();
ERTotal : number = 0;
DDTotal : number = 0;
Total : number = 0;
Memo : Memo[] = new Array<Memo>();
}
