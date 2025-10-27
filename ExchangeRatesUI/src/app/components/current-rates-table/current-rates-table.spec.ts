import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentRatesTable } from './current-rates-table';

describe('CurrentRatesTable', () => {
  let component: CurrentRatesTable;
  let fixture: ComponentFixture<CurrentRatesTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CurrentRatesTable]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrentRatesTable);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
