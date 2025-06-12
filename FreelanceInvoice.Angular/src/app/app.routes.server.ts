import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: '',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'auth',
    renderMode: RenderMode.Prerender
  },
  {
    path: 'invoices/:id',
    renderMode: RenderMode.Prerender,
    getPrerenderParams: () => Promise.resolve([{ id: '1' }])
  },
  {
    path: 'invoices/:id/edit',
    renderMode: RenderMode.Prerender,
    getPrerenderParams: () => Promise.resolve([{ id: '1' }])
  },
  {
    path: 'clients/:id/edit',
    renderMode: RenderMode.Prerender,
    getPrerenderParams: () => Promise.resolve([{ id: '1' }])
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
