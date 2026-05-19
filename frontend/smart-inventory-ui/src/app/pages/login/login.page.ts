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
import { BRAND } from '../../core/brand.config';
import { sanitizeReturnUrl } from '../../core/navigation/return-url.util';

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

  readonly brand = BRAND;
  readonly hidePassword = signal(true);

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  get username() {
    return this.form.controls.username;
  }

  get password() {
    return this.form.controls.password;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, password } = this.form.getRawValue();

    this.auth
      .login({ username, password })
      .subscribe({
        next: () => {
          if (!this.auth.isLoggedIn()) {
            this.notifications.error(
              'Sign-in succeeded but the session could not be started. Try again.',
              'Login failed',
            );
            return;
          }

          const returnUrl = sanitizeReturnUrl(
            this.route.snapshot.queryParamMap.get('returnUrl'),
          );

          void this.router
            .navigateByUrl(returnUrl, { replaceUrl: true })
            .then((navigated) => {
              if (!navigated) {
                void this.router.navigate(['/home'], { replaceUrl: true });
              }
              this.notifications.success('Welcome back!', 'Login successful');
            });
        },
        error: () => {
          this.notifications.error('Invalid username or password.', 'Login failed');
        },
      });
  }

  togglePasswordVisibility(): void {
    this.hidePassword.update((value) => !value);
  }
}
