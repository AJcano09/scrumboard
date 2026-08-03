import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {ButtonModule} from "primeng/button";
import {InputTextModule} from "primeng/inputtext";

@Component({
  selector: "app-board",
  standalone: true,
  imports:
  [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule
  ],
  templateUrl: "./board.component.html",
})
export class BoardComponent{}
