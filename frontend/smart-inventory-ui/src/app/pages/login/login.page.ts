import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { LoadingButton } from '../../shared';
import { NotificationService } from '../../core/notifications/notification.service';
import { AuthService } from '../../features/auth/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    LoadingButton,
  ],
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss',
})
export class LoginPage {
  private readonly fb = inject(FormBuilder);
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notifications = inject(NotificationService);

  readonly appName = environment.appName;
  readonly hidePassword = signal(true);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  get email() {
    return this.form.controls.email;
  }

  get password() {
    return this.form.controls.password;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password } = this.form.getRawValue();

    this.auth
      .login({ email, password })
      .subscribe({
        next: () => {
          this.notifications.success('Welcome back!', 'Login successful');
          const returnUrl =
            this.route.snapshot.queryParamMap.get('returnUrl') ?? '/dashboard';
          void this.router.navigateByUrl(returnUrl);
        },
        error: () => {
          this.notifications.error('Invalid email or password.', 'Login failed');
        },
      });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update((value) => !value);
  }
}
