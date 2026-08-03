import {Component} from "@angular/core";
import {CommonModule} from "@angular/common";
import {FormBuilder, ReactiveFormsModule, Validators} from "@angular/forms";
import {InputTextModule} from "primeng/inputtext";
import {PasswordModule} from "primeng/password";
import {MessageModule} from "primeng/message";
import {CardModule} from "primeng/card";
import {ButtonModule} from "primeng/button";
import {AuthService} from "../../core/auth/auth.service";
import {Router} from "@angular/router";

@Component(
  {
    selector: 'app-login',
    standalone:true,
    imports:[
      CommonModule,ReactiveFormsModule,
      ButtonModule,InputTextModule,PasswordModule,MessageModule,CardModule
    ],
    templateUrl:'login.component.html'
  }
)
export class LoginComponent {
  form =this.fb.group({
    email:['', Validators.required, Validators.email],
    password:['', Validators.required]
  });

  errorMessage ='';
  loading =false;

  constructor(private fb:FormBuilder,
              private auth:AuthService,
              private router:Router,
              ) {
  }

  submit():void{
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }
    this.loading =true;
    this.errorMessage= '';

    const {email, password} = this.form.getRawValue();
    this.auth.login(email!,password!).subscribe({
      next: ()=> this.router.navigateByUrl('/board'),
      error:()=> {
        this.errorMessage = 'Credenciales inválidas';
        this.loading = false;
      },
    })

  }
}
