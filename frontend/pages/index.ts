import { createRoutesView } from 'atomic-router-react';
import { routes } from '~/shared/routing';
import { RootPage, rootRoute } from './root';

import { ProfileView } from '~/pages/profile';
import { AccountView } from '~/pages/profile-account';
import { profileAccountRoute } from '~/pages/profile-account/model.ts';
import { FinancesView } from '~/pages/profile-finances';
import { profileFinancesRoute } from '~/pages/profile-finances/model.ts';
import { ProductsView } from '~/pages/profile-products';
import { profileProductsNewRoute } from '~/pages/profile-products-new/model.ts';
import { profileProductsRoute } from '~/pages/profile-products/model.ts';
import { SubscriptionsView } from '~/pages/profile-subscriptions';
import { profileSubscriptionsRoute } from '~/pages/profile-subscriptions/model.ts';
import { ProfileLayout } from '~/shared/ui/layout/profile';
import { ErrorPage } from './error';
import { DocsPage, docsRoute } from './legal-docs';
import { LoginPage, loginRoute, signupRoute } from './login';
import { profileRoute } from './profile/model';
import { ProductPage, productRoute } from './product-сard';
import { TodosPage, todosRoute } from './todos';
import { DocsLayout } from '~/shared/ui/layout/docs';
import { resetpassRoute } from './login/model';
import { errorRoute } from './error/model';
import { ProductsNew } from './profile-products-new';

const RoutesView = createRoutesView({
  routes: [
    { route: rootRoute, view: RootPage },
    { route: profileRoute, view: ProfileView, layout: ProfileLayout },
    {
      route: profileAccountRoute,
      view: AccountView,
      layout: ProfileLayout,
    },
    {
      route: profileProductsRoute,
      view: ProductsView,
      layout: ProfileLayout,
    },
    {
      route: profileProductsNewRoute,
      view: ProductsNew,
      layout: ProfileLayout,
    },
    {
      route: profileFinancesRoute,
      view: FinancesView,
      layout: ProfileLayout,
    },
    {
      route: profileSubscriptionsRoute,
      view: SubscriptionsView,
      layout: ProfileLayout,
    },
    { route: loginRoute, view: LoginPage },
    { route: signupRoute, view: LoginPage },
    { route: resetpassRoute, view: LoginPage },
    { route: todosRoute, view: TodosPage },
    { route: productRoute, view: ProductPage },
    { route: docsRoute, view: DocsPage, layout: DocsLayout },
    { route: errorRoute, view: ErrorPage },
  ],
});

export const Pages = RoutesView;
