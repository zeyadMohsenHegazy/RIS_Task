import {
  Component,
  computed,
  DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { NotificationService } from '../../core/notifications/notification.service';
import { finalize } from 'rxjs';
import { AuthService } from '../../features/auth/auth.service';
import { UsersService } from '../../features/users/users.service';
import { UsersStore } from '../../features/users/users.store';
import { USER_ROLES } from '../../models/user.model';
import { ErrorState, FormSkeleton, LoadingButton, PageHeader } from '../../shared';

@Component({
  selector: 'app-user-form-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    ErrorState,
    FormSkeleton,
    LoadingButton,
    PageHeader,
  ],
  templateUrl: './user-form.page.html',
  styleUrl: './user-form.page.scss',
})
export class UserFormPage implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly usersService = inject(UsersService);
  private readonly usersStore = inject(UsersStore);
  private readonly auth = inject(AuthService);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  private readonly userIdParam = this.route.snapshot.paramMap.get('id');
  readonly userId = this.userIdParam ? Number(this.userIdParam) : null;
  readonly isEditMode = computed(() => this.userId != null && !Number.isNaN(this.userId));
  readonly roles = USER_ROLES;

  readonly userLoading = signal(false);
  readonly submitting = signal(false);
  readonly loadError = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.maxLength(100)]],
    role: ['Employee', [Validators.required]],
    password: ['', [Validators.minLength(6)]],
  });

  readonly pageTitle = computed(() =>
    this.isEditMode() ? 'Edit User' : 'Add User',
  );

  ngOnInit(): void {
    if (!this.auth.isAdmin()) {
      this.notifications.warning('You do not have permission to manage users.');
      void this.router.navigate(['/home']);
      return;
    }

    if (this.isEditMode()) {
      this.form.controls.password.clearValidators();
      this.form.controls.password.updateValueAndValidity();
      this.loadUser();
      return;
    }

    this.form.controls.password.setValidators([
      Validators.required,
      Validators.minLength(6),
    ]);
    this.form.controls.password.updateValueAndValidity();
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { username, role, password } = this.form.getRawValue();
    this.submitting.set(true);

    const request$ = this.isEditMode()
      ? this.usersService.update(this.userId!, {
          username,
          role,
          password: password.trim() ? password : null,
        })
      : this.usersService.create({ username, role, password });

    request$
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.notifications.success(
            this.isEditMode() ? 'User updated successfully.' : 'User created successfully.',
          );
          void this.router.navigate(['/users']).then(() => {
            this.usersStore.invalidateList();
          });
        },
      });
  }

  get username() {
    return this.form.controls.username;
  }

  get role() {
    return this.form.controls.role;
  }

  get password() {
    return this.form.controls.password;
  }

  retryLoad(): void {
    this.loadError.set(null);
    this.loadUser();
  }

  private loadUser(): void {
    if (this.userId == null || Number.isNaN(this.userId)) {
      this.loadError.set('Invalid user id.');
      return;
    }

    this.userLoading.set(true);
    this.usersService
      .getById(this.userId)
      .pipe(
        finalize(() => this.userLoading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (user) => {
          this.form.patchValue({
            username: user.username,
            role: user.role,
            password: '',
          });
        },
        error: () => {
          this.loadError.set('User not found or could not be loaded.');
        },
      });
  }
}
