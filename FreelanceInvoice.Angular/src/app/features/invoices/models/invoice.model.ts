export interface InvoiceItem {
  id?: number;
  description: string;
  quantity: number;
  rate: number;
  tax: number;
  amount: number;
}

export interface Invoice {
  id?: number;
  number: string;
  clientId: number;
  clientName: string;
  issueDate: Date;
  dueDate: Date;
  status: 'Draft'| 'Pending' | 'Sent' | 'Paid' | 'Overdue' | 'Cancelled';
  items: InvoiceItem[];
  subtotal: number;
  tax: number;
  total: number;
  notes?: string;
  terms?: string;
}

export interface CreateInvoiceDto {
  clientId: number;
  issueDate: Date;
  dueDate: Date;
  items: Omit<InvoiceItem, 'id' | 'amount'>[];
  notes?: string;
  terms?: string;
}

export interface UpdateInvoiceDto extends Partial<CreateInvoiceDto> {
  status?: Invoice['status'];
} 