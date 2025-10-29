import { chainAuthorized } from '~/shared/auth.ts';
import { routes } from '~/shared/routing.ts';

export const profileSubscriptionsRoute = chainAuthorized(routes.profile.subscriptions);
