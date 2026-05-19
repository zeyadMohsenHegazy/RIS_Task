import { Component, computed, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageEvent } from '@angular/material/paginator';
import { filter, switchMap, tap } from 'rxjs';
import { NotificationService } from '../../core/notifications/notification.service';
import { AuthService } from '../../features/auth/auth.service';
import { UsersService } from '../../features/users/users.service';
import { UsersStore } from '../../features/users/users.store';
import { UserDto } from '../../models/user.model';
import {
  ConfirmDialogService,
  ErrorState,
  PageHeader,
  PaginatedTableShell,
  PAGE_SIZE_OPTIONS,
  SearchField,
  bindDebouncedSearch,
} from '../../shared';

@Component({
  selector: 'app-users-list-page',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    PageHeader,
    SearchField,
    PaginatedTableShell,
    ErrorState,
  ],
  templateUrl: './users-list.page.html',
  styleUrl: './users-list.page.scss',
})
export class UsersListPage implements OnInit {
  private readonly usersStore = inject(UsersStore);
  private readonly usersService = inject(UsersService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notifications = inject(NotificationService);
  private readonly destroyRef = inject(DestroyRef);

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly users = this.usersStore.users;
  readonly totalCount = this.usersStore.totalCount;
  readonly loading = this.usersStore.listLoading;
  readonly error = this.usersStore.listError;
  readonly currentUserId = computed(() => this.auth.currentUser()?.id ?? null);

  readonly pageIndex = computed(() => this.usersStore.listQuery().pageNumber - 1);
  readonly pageSize = computed(() => this.usersStore.listQuery().pageSize);
  readonly displayedColumns = ['username', 'role', 'actions'] as const;

  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

  ngOnInit(): void {
    if (!this.auth.isAdmin()) {
      this.notifications.warning('You do not have permission to manage users.');
      void this.router.navigate(['/home']);
      return;
    }

    this.searchControl.setValue(this.usersStore.listQuery().search, {
      emitEvent: false,
    });
    bindDebouncedSearch(this.searchControl, this.destroyRef, (search) => {
      this.usersStore.setListQuery({ pageNumber: 1, search });
    });
    this.usersStore.ensureListLoaded();
  }

  onPageChange(event: PageEvent): void {
    this.usersStore.setListQuery({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    });
  }

  loadUsers(): void {
    this.usersStore.loadList();
  }

  onAddUser(): void {
    void this.router.navigate(['/users/new']);
  }

  onEditUser(user: UserDto): void {
    void this.router.navigate(['/users', user.id, 'edit']);
  }

  isCurrentUser(user: UserDto): boolean {
    const currentId = this.currentUserId();
    return currentId != null && user.id === currentId;
  }

  onDeleteUser(user: UserDto): void {
    const deletingSelf = this.isCurrentUser(user);

    this.confirmDialog
      .confirm({
        title: deletingSelf ? 'Delete your account' : 'Delete user',
        message: deletingSelf
          ? 'You are about to delete your own account. You will be logged out immediately. This action cannot be undone.'
          : `Are you sure you want to delete "${user.username}"? This action cannot be undone.`,
        confirmText: deletingSelf ? 'Delete and log out' : 'Delete',
        cancelText: 'Cancel',
        confirmColor: 'warn',
      })
      .pipe(
        filter((confirmed) => confirmed === true),
        switchMap(() =>
          this.usersService.delete(user.id).pipe(
            tap(() => {
              if (deletingSelf) {
                this.notifications.success('Your account was deleted.');
                this.auth.logout();
              } else {
                this.notifications.success(`"${user.username}" was deleted.`);
              }
            }),
          ),
        ),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          if (!deletingSelf) {
            this.usersStore.invalidateList();
          }
        },
      });
  }
}
