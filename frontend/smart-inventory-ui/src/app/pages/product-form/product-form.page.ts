import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-product-form-page',
  imports: [MatCardModule, MatButtonModule, RouterLink],
  templateUrl: './product-form.page.html',
  styleUrl: './product-form.page.scss',
})
export class ProductFormPage {
  private readonly route = inject(ActivatedRoute);
  readonly productId = this.route.snapshot.paramMap.get('id');
  readonly isEditMode = !!this.productId;
}
