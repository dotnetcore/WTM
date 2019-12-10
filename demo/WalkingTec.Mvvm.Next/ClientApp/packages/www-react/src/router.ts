import { renderRoutes, RouteConfig } from 'react-router-config';
import React from 'react';
const routes = [
  {
    path: '/',
    name: 'home',
    exact: true,
    component: React.lazy(() => import('./views/home'))
  },
  {
    path: '/about',
    name: 'about',
    exact: true,
    component: React.lazy(() => import('./views/about'))
  }
]

export default renderRoutes(routes)
