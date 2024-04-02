import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegistrationService } from '../../Services/registration.service';
import { RegisterDto } from '../../ViewModels/register-dto';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.css'
})
export class RegistrationComponent {
  user:RegisterDto={} as RegisterDto
  constructor( private _registrationService :RegistrationService )
  {

  }
  register(username: string, email: string, password: string, confirmpass: string) {
    this._registrationService.register(username, email, password, confirmpass).subscribe({
      next: (res) => {
        this.user.userName = username;
        localStorage.setItem("username", username);

        this.user.email = email;
        localStorage.setItem("email", email);

        this.user.password = password; 
        localStorage.setItem("password", password);

        this.user.confirmpass = confirmpass;
        localStorage.setItem("confirmpass", confirmpass);

        console.log(res);
      },
      error: (err) => {
        console.error(err);
      }
    });

}}