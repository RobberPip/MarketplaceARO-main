import { chainAuthorized } from '~/shared/auth.ts';
import { routes } from '~/shared/routing.ts';

export const profileFinancesRoute = chainAuthorized(routes.profile.finances);
