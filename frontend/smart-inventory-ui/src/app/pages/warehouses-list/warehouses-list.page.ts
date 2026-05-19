import { Component, inject, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { WarehousesStore } from '../../features/warehouses/warehouses.store';
import { ErrorState, PageHeader, TableSkeleton } from '../../shared';

@Component({
  selector: 'app-warehouses-list-page',
  imports: [MatCardModule, MatTableModule, PageHeader, ErrorState, TableSkeleton],
  templateUrl: './warehouses-list.page.html',
  styleUrl: './warehouses-list.page.scss',
})
export class WarehousesListPage implements OnInit {
  private readonly warehousesStore = inject(WarehousesStore);

  readonly warehouses = this.warehousesStore.warehouses;
  readonly loading = this.warehousesStore.loading;
  readonly error = this.warehousesStore.error;

  readonly displayedColumns = ['name', 'location'];

  ngOnInit(): void {
    this.warehousesStore.load();
  }

  reload(): void {
    this.warehousesStore.load(true);
  }
}
