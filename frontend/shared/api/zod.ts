import { z } from 'zod';

export type GetPublicListOut = z.infer<typeof GetPublicListOut>;
export const GetPublicListOut = z.object({
  id: z.string().optional(),
  name: z.union([z.string(), z.null()]).optional(),
});

export type GetMainActivityOut = z.infer<typeof GetMainActivityOut>;
export const GetMainActivityOut = z.object({
  id: z.string().optional(),
  name: z.union([z.string(), z.null()]).optional(),
});

export type AccountOptionsOut = z.infer<typeof AccountOptionsOut>;
export const AccountOptionsOut = z.object({
  publicLists: z.array(GetPublicListOut),
  mainActivities: z.array(GetMainActivityOut),
});

export type AccountRole = z.infer<typeof AccountRole>;
export const AccountRole = z.union([
  z.literal('Owner'),
  z.literal('Manager'),
  z.literal('Developer'),
  z.literal('Member'),
]);

export type AccountRoleUpdateItem = z.infer<typeof AccountRoleUpdateItem>;
export const AccountRoleUpdateItem = z.object({
  accountProfileId: z.string().optional(),
  role: AccountRole.optional(),
});

export type AccountStatus = z.infer<typeof AccountStatus>;
export const AccountStatus = z.union([z.literal('Approved'), z.literal('Pending'), z.literal('Blocked')]);

export type AccountType = z.infer<typeof AccountType>;
export const AccountType = z.union([
  z.literal('Regular'),
  z.literal('SelfEmployed'),
  z.literal('IndividualEntrepreneur'),
  z.literal('LegalEntity'),
  z.literal('LimitedLiabilityCompany'),
  z.literal('JointStockCompany'),
  z.literal('NonProfitOrganization'),
  z.literal('Individual'),
  z.literal('GovernmentInstitution'),
  z.literal('PrivateEnterprise'),
  z.literal('ForeignOrganization'),
  z.literal('ForeignIndividual'),
]);

export type AvatarOut = z.infer<typeof AvatarOut>;
export const AvatarOut = z.object({
  url: z.union([z.string(), z.null()]).optional(),
  updatedAt: z.union([z.string(), z.null()]).optional(),
});

export type BrandDto = z.infer<typeof BrandDto>;
export const BrandDto = z.object({
  id: z.union([z.string(), z.null(), z.undefined()]).optional(),
  avatar: AvatarOut,
  publicName: z.union([z.string(), z.null()]),
});

export type AccountsDto = z.infer<typeof AccountsDto>;
export const AccountsDto = z.object({
  id: z.union([z.string(), z.null(), z.undefined()]).optional(),
  inn: z.union([z.string(), z.null()]),
  fullName: z.union([z.string(), z.null()]),
  shortName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  userRole: AccountRole,
  type: AccountType,
  brand: BrandDto,
});

export type UpdateBrandIn = z.infer<typeof UpdateBrandIn>;
export const UpdateBrandIn = z.object({
  publicName: z.union([z.string(), z.null()]),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  website: z.union([z.string(), z.null(), z.undefined()]).optional(),
  email: z.union([z.string(), z.null(), z.undefined()]).optional(),
  phoneNumber: z.union([z.string(), z.null(), z.undefined()]).optional(),
  publicListIds: z.union([z.array(z.string()), z.null(), z.undefined()]).optional(),
  mainActivityIds: z.union([z.array(z.string()), z.null(), z.undefined()]).optional(),
});

export type AddAccountIn = z.infer<typeof AddAccountIn>;
export const AddAccountIn = z.object({
  type: AccountType,
  isDeveloper: z.union([z.boolean(), z.null(), z.undefined()]).optional(),
  inn: z.union([z.string(), z.null()]),
  fullName: z.union([z.string(), z.null()]),
  shortName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  address: z.union([z.string(), z.null(), z.undefined()]).optional(),
  kpp: z.union([z.string(), z.null(), z.undefined()]).optional(),
  ogrn: z.union([z.string(), z.null(), z.undefined()]).optional(),
  brand: UpdateBrandIn,
});

export type ProductType = z.infer<typeof ProductType>;
export const ProductType = z.union([
  z.literal('Software'),
  z.literal('Plugin'),
  z.literal('Extension'),
  z.literal('Analog'),
  z.literal('Command'),
  z.literal('Function'),
]);

export type AddProductDraftIn = z.infer<typeof AddProductDraftIn>;
export const AddProductDraftIn = z.object({
  name: z.union([z.string(), z.null()]),
  type: ProductType,
});

export type BrandStatus = z.infer<typeof BrandStatus>;
export const BrandStatus = z.union([z.literal('Approved'), z.literal('Pending'), z.literal('Blocked')]);

export type ChangeLoginDto = z.infer<typeof ChangeLoginDto>;
export const ChangeLoginDto = z.object({
  login: z.string(),
});

export type ChangePasswordDto = z.infer<typeof ChangePasswordDto>;
export const ChangePasswordDto = z.object({
  currentPassword: z.string(),
  newPassword: z.string(),
});

export type CompanyStatus = z.infer<typeof CompanyStatus>;
export const CompanyStatus = z.union([z.literal('Init'), z.literal('Approved'), z.literal('Pending')]);

export type CompanyOut = z.infer<typeof CompanyOut>;
export const CompanyOut = z.object({
  id: z.string(),
  name: z.union([z.string(), z.null(), z.undefined()]).optional(),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  status: CompanyStatus,
  isRegistry: z.boolean(),
  createdAt: z.union([z.string(), z.undefined()]).optional(),
  updatedAt: z.union([z.string(), z.undefined()]).optional(),
});

export type CurrentUserInfoDto = z.infer<typeof CurrentUserInfoDto>;
export const CurrentUserInfoDto = z.object({
  id: z.union([z.string(), z.undefined()]).optional(),
  userName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  email: z.union([z.string(), z.null(), z.undefined()]).optional(),
  login: z.union([z.string(), z.null(), z.undefined()]).optional(),
  avatar: z.union([z.string(), z.null(), z.undefined()]).optional(),
  firstName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  lastName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  emailConfirmed: z.union([z.boolean(), z.undefined()]).optional(),
  twoFactorEnabled: z.union([z.boolean(), z.undefined()]).optional(),
  lockout: z.union([z.boolean(), z.undefined()]).optional(),
  accessFailedCount: z.union([z.number(), z.undefined()]).optional(),
  roles: z.union([z.array(z.string()), z.null(), z.undefined()]).optional(),
  accounts: z.union([z.array(AccountsDto), z.null()]),
  activeAccountId: z.union([z.string(), z.null()]),
  isDevStatus: z.union([z.boolean(), z.null(), z.undefined()]).optional(),
});

export type EmailDto = z.infer<typeof EmailDto>;
export const EmailDto = z.object({
  email: z.string(),
});

export type ErrorResponse = z.infer<typeof ErrorResponse>;
export const ErrorResponse = z.object({
  title: z.union([z.string(), z.null()]).optional(),
  status: z.union([z.number(), z.null()]).optional(),
  detail: z.union([z.string(), z.null()]).optional(),
  errors: z.union([z.unknown(), z.null()]).optional(),
});

export type ForgotPasswordRequest = z.infer<typeof ForgotPasswordRequest>;
export const ForgotPasswordRequest = z.object({
  email: z.union([z.string(), z.null()]),
});

export type InvitationStatus = z.infer<typeof InvitationStatus>;
export const InvitationStatus = z.union([
  z.literal('Invited'),
  z.literal('Accepted'),
  z.literal('Revoked'),
  z.literal('Rejected'),
]);

export type ProfileRoleOut = z.infer<typeof ProfileRoleOut>;
export const ProfileRoleOut = z.object({
  id: z.string().optional(),
  name: z.union([z.string(), z.null()]).optional(),
  category: z.union([z.string(), z.null()]).optional(),
});

export type ProfileIndustryOut = z.infer<typeof ProfileIndustryOut>;
export const ProfileIndustryOut = z.object({
  id: z.string().optional(),
  name: z.union([z.string(), z.null()]).optional(),
});

export type ProfileCompanyOut = z.infer<typeof ProfileCompanyOut>;
export const ProfileCompanyOut = z.object({
  id: z.string(),
  name: z.union([z.string(), z.null()]),
  status: CompanyStatus,
});

export type ProfileOptionsOut = z.infer<typeof ProfileOptionsOut>;
export const ProfileOptionsOut = z.object({
  roles: z.array(ProfileRoleOut),
  industries: z.array(ProfileIndustryOut),
  companies: z.array(ProfileCompanyOut),
});

export type UserProfileOut = z.infer<typeof UserProfileOut>;
export const UserProfileOut = z.object({
  userId: z.string(),
  userEmail: z.union([z.string(), z.null()]),
  login: z.union([z.string(), z.null(), z.undefined()]).optional(),
  firstName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  lastName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  avatar: AvatarOut,
  options: z.union([ProfileOptionsOut, z.undefined()]).optional(),
  company: z.union([ProfileCompanyOut, z.undefined()]).optional(),
  industry: z.union([ProfileIndustryOut, z.undefined()]).optional(),
  role: z.union([ProfileRoleOut, z.undefined()]).optional(),
  updatedAt: z.union([z.string(), z.null()]),
  createdAt: z.string(),
});

export type InvitationsOut = z.infer<typeof InvitationsOut>;
export const InvitationsOut = z.object({
  id: z.string().optional(),
  email: z.union([z.string(), z.null()]).optional(),
  role: AccountRole.optional(),
  status: InvitationStatus.optional(),
  profile: UserProfileOut.optional(),
  createdAt: z.string().optional(),
  acceptedAt: z.union([z.string(), z.null()]).optional(),
});

export type GetBrandOut = z.infer<typeof GetBrandOut>;
export const GetBrandOut = z.object({
  id: z.union([z.string(), z.undefined()]).optional(),
  status: BrandStatus,
  avatar: AvatarOut,
  publicName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  mainActivities: z.union([z.array(GetMainActivityOut), z.null(), z.undefined()]).optional(),
  website: z.union([z.string(), z.null(), z.undefined()]).optional(),
  email: z.union([z.string(), z.null(), z.undefined()]).optional(),
  phoneNumber: z.union([z.string(), z.null(), z.undefined()]).optional(),
  createdAt: z.union([z.string(), z.undefined()]).optional(),
  updatedAt: z.union([z.string(), z.null(), z.undefined()]).optional(),
  publicLists: z.union([z.array(GetPublicListOut), z.null(), z.undefined()]).optional(),
});

export type GetAccountOut = z.infer<typeof GetAccountOut>;
export const GetAccountOut = z.object({
  id: z.union([z.string(), z.null(), z.undefined()]).optional(),
  status: z.union([AccountStatus, z.undefined()]).optional(),
  isDeveloper: z.boolean(),
  type: AccountType,
  inn: z.union([z.string(), z.null()]),
  shortName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  fullName: z.union([z.string(), z.null()]),
  options: AccountOptionsOut,
  address: z.union([z.string(), z.null(), z.undefined()]).optional(),
  kpp: z.union([z.string(), z.null(), z.undefined()]).optional(),
  ogrn: z.union([z.string(), z.null(), z.undefined()]).optional(),
  invitations: z.union([z.array(InvitationsOut), z.null(), z.undefined()]).optional(),
  userRole: z.union([AccountRole, z.undefined()]).optional(),
  brand: z.union([GetBrandOut, z.undefined()]).optional(),
  createdAt: z.union([z.string(), z.undefined()]).optional(),
  updatedAt: z.union([z.string(), z.null(), z.undefined()]).optional(),
});

export type GetAttributesDataOut = z.infer<typeof GetAttributesDataOut>;
export const GetAttributesDataOut = z.object({
  attributeId: z.string().optional(),
  attributeName: z.union([z.string(), z.null()]).optional(),
  dataType: z.union([z.string(), z.null()]).optional(),
  value: z.union([z.unknown(), z.null()]).optional(),
});

export type GetCompanyOut = z.infer<typeof GetCompanyOut>;
export const GetCompanyOut = z.object({
  inn: z.union([z.string(), z.null()]).optional(),
  ogrn: z.union([z.string(), z.null()]).optional(),
  shortName: z.union([z.string(), z.null()]).optional(),
  fullName: z.union([z.string(), z.null()]).optional(),
  registrationDate: z.union([z.string(), z.null()]).optional(),
  terminationDate: z.union([z.string(), z.null()]).optional(),
  fullAddress: z.union([z.string(), z.null()]).optional(),
});

export type GetProductOut = z.infer<typeof GetProductOut>;
export const GetProductOut = z.object({
  id: z.union([z.string(), z.undefined()]).optional(),
  productData: z.union([z.array(GetAttributesDataOut), z.null(), z.undefined()]).optional(),
  name: z.union([z.string(), z.null()]),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  website: z.union([z.string(), z.null(), z.undefined()]).optional(),
  type: ProductType,
  avatar: AvatarOut,
  createdAt: z.union([z.string(), z.null(), z.undefined()]).optional(),
  updatedAt: z.union([z.string(), z.null(), z.undefined()]).optional(),
});

export type HttpValidationProblemDetails = z.infer<typeof HttpValidationProblemDetails>;
export const HttpValidationProblemDetails = z.intersection(
  z.object({
    type: z.union([z.string(), z.null()]).optional(),
    title: z.union([z.string(), z.null()]).optional(),
    status: z.union([z.number(), z.null()]).optional(),
    detail: z.union([z.string(), z.null()]).optional(),
    instance: z.union([z.string(), z.null()]).optional(),
    errors: z.union([z.unknown(), z.null()]).optional(),
  }),
  z.object({
    string: z.any().optional(),
  }),
);

export type InfoRequest = z.infer<typeof InfoRequest>;
export const InfoRequest = z.object({
  newEmail: z.union([z.string(), z.null()]).optional(),
  newPassword: z.union([z.string(), z.null()]).optional(),
  oldPassword: z.union([z.string(), z.null()]).optional(),
});

export type InfoResponse = z.infer<typeof InfoResponse>;
export const InfoResponse = z.object({
  email: z.union([z.string(), z.null()]),
  isEmailConfirmed: z.boolean(),
});

export type LoginDto = z.infer<typeof LoginDto>;
export const LoginDto = z.object({
  email: z.string(),
  password: z.string(),
  rememberMe: z.boolean(),
  data: z.union([z.unknown(), z.null(), z.undefined()]).optional(),
});

export type RegistrationDto = z.infer<typeof RegistrationDto>;
export const RegistrationDto = z.object({
  email: z.string(),
  password: z.string(),
  checkedAgreements: z.boolean(),
  checkedRules: z.boolean(),
  data: z.union([z.unknown(), z.null(), z.undefined()]).optional(),
});

export type ResetPasswordDto = z.infer<typeof ResetPasswordDto>;
export const ResetPasswordDto = z.object({
  userId: z.string(),
  token: z.string(),
  newPassword: z.string(),
  confirmNewPassword: z.string(),
});

export type ResetPasswordRequest = z.infer<typeof ResetPasswordRequest>;
export const ResetPasswordRequest = z.object({
  email: z.union([z.string(), z.null()]),
  resetCode: z.union([z.string(), z.null()]),
  newPassword: z.union([z.string(), z.null()]),
});

export type SendConfirmEmailDto = z.infer<typeof SendConfirmEmailDto>;
export const SendConfirmEmailDto = z.object({
  email: z.string(),
});

export type UpdateAccountIn = z.infer<typeof UpdateAccountIn>;
export const UpdateAccountIn = z.object({
  type: AccountType,
  inn: z.union([z.string(), z.null()]),
  fullName: z.union([z.string(), z.null()]),
  shortName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  address: z.union([z.string(), z.null(), z.undefined()]).optional(),
  kpp: z.union([z.string(), z.null(), z.undefined()]).optional(),
  ogrn: z.union([z.string(), z.null(), z.undefined()]).optional(),
});

export type UpdateProductIn = z.infer<typeof UpdateProductIn>;
export const UpdateProductIn = z.object({
  description: z.union([z.string(), z.null()]).optional(),
  website: z.union([z.string(), z.null()]).optional(),
  rightholders: z.union([z.array(z.string()), z.null()]).optional(),
  productData: z.union([z.unknown(), z.null()]).optional(),
});

export type UpdateProfileIn = z.infer<typeof UpdateProfileIn>;
export const UpdateProfileIn = z.object({
  firstName: z.union([z.string(), z.null()]).optional(),
  lastName: z.union([z.string(), z.null()]).optional(),
  login: z.union([z.string(), z.null()]).optional(),
  description: z.union([z.string(), z.null()]).optional(),
  companyId: z.union([z.string(), z.null()]).optional(),
  industryId: z.union([z.string(), z.null()]).optional(),
  roleId: z.union([z.string(), z.null()]).optional(),
  companyName: z.union([z.string(), z.null()]).optional(),
});

export type UpdateProfileOut = z.infer<typeof UpdateProfileOut>;
export const UpdateProfileOut = z.object({
  firstName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  lastName: z.union([z.string(), z.null(), z.undefined()]).optional(),
  login: z.union([z.string(), z.null(), z.undefined()]).optional(),
  description: z.union([z.string(), z.null(), z.undefined()]).optional(),
  company: z.union([ProfileCompanyOut, z.undefined()]).optional(),
  industry: z.union([ProfileIndustryOut, z.undefined()]).optional(),
  role: z.union([ProfileRoleOut, z.undefined()]).optional(),
  userId: z.string(),
  profileId: z.string(),
  userEmail: z.union([z.string(), z.null()]),
  updatedAt: z.string(),
  createdAt: z.string(),
});

export type ValidationProblemDetails = z.infer<typeof ValidationProblemDetails>;
export const ValidationProblemDetails = z.intersection(
  z.object({
    type: z.union([z.string(), z.null()]).optional(),
    title: z.union([z.string(), z.null()]).optional(),
    status: z.union([z.number(), z.null()]).optional(),
    detail: z.union([z.string(), z.null()]).optional(),
    instance: z.union([z.string(), z.null()]).optional(),
    errors: z.union([z.unknown(), z.null()]).optional(),
  }),
  z.object({
    string: z.any().optional(),
  }),
);

export type Void = z.infer<typeof Void>;
export const Void = z.unknown();

export type get_Apiv1account = typeof get_Apiv1account;
export const get_Apiv1account = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/account'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: GetAccountOut,
};

export type post_Apiv1account = typeof post_Apiv1account;
export const post_Apiv1account = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/account'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: AddAccountIn,
  }),
  response: GetAccountOut,
};

export type put_Apiv1accountAccountId = typeof put_Apiv1accountAccountId;
export const put_Apiv1accountAccountId = {
  method: z.literal('PUT'),
  path: z.literal('/api/v1/account/{accountId}'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    path: z.object({
      accountId: z.string(),
    }),
    body: UpdateAccountIn,
  }),
  response: GetAccountOut,
};

export type patch_Apiv1accountAccountIddeveloper = typeof patch_Apiv1accountAccountIddeveloper;
export const patch_Apiv1accountAccountIddeveloper = {
  method: z.literal('PATCH'),
  path: z.literal('/api/v1/account/{accountId}/developer'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      isDeveloper: z.boolean(),
    }),
    path: z.object({
      accountId: z.string(),
    }),
  }),
  response: GetAccountOut,
};

export type patch_Apiv1brand = typeof patch_Apiv1brand;
export const patch_Apiv1brand = {
  method: z.literal('PATCH'),
  path: z.literal('/api/v1/brand'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: UpdateBrandIn,
  }),
  response: GetBrandOut,
};

export type put_Apiv1activeAccount = typeof put_Apiv1activeAccount;
export const put_Apiv1activeAccount = {
  method: z.literal('PUT'),
  path: z.literal('/api/v1/active-account'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      accountId: z.string().optional(),
    }),
  }),
  response: GetAccountOut,
};

export type get_Apiv1accountoptions = typeof get_Apiv1accountoptions;
export const get_Apiv1accountoptions = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/account/options'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: AccountOptionsOut,
};

export type post_Apiv1brandavatar = typeof post_Apiv1brandavatar;
export const post_Apiv1brandavatar = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/brand/avatar'),
  requestFormat: z.literal('form-data'),
  parameters: z.object({
    body: z.object({
      imageFile: z.string(),
    }),
  }),
  response: z.unknown(),
};

export type delete_Apiv1brandavatar = typeof delete_Apiv1brandavatar;
export const delete_Apiv1brandavatar = {
  method: z.literal('DELETE'),
  path: z.literal('/api/v1/brand/avatar'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: z.unknown(),
};

export type post_Apiv1accountinvitationinvite = typeof post_Apiv1accountinvitationinvite;
export const post_Apiv1accountinvitationinvite = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/account/invitation/invite'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      email: z.string(),
      role: z.union([z.literal('Owner'), z.literal('Manager'), z.literal('Developer'), z.literal('Member')]),
    }),
  }),
  response: Void,
};

export type get_Apiv1accountinvitationinviteAccept = typeof get_Apiv1accountinvitationinviteAccept;
export const get_Apiv1accountinvitationinviteAccept = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/account/invitation/invite-accept'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      invitationId: z.string(),
      token: z.string(),
    }),
  }),
  response: Void,
};

export type put_Apiv1accountinvitationupdateRoles = typeof put_Apiv1accountinvitationupdateRoles;
export const put_Apiv1accountinvitationupdateRoles = {
  method: z.literal('PUT'),
  path: z.literal('/api/v1/account/invitation/update-roles'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: z.array(AccountRoleUpdateItem),
  }),
  response: Void,
};

export type delete_Apiv1accountinvitationremove = typeof delete_Apiv1accountinvitationremove;
export const delete_Apiv1accountinvitationremove = {
  method: z.literal('DELETE'),
  path: z.literal('/api/v1/account/invitation/remove'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      invitationId: z.string(),
    }),
  }),
  response: Void,
};

export type post_Authlogin = typeof post_Authlogin;
export const post_Authlogin = {
  method: z.literal('POST'),
  path: z.literal('/auth/login'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: LoginDto,
  }),
  response: z.unknown(),
};

export type post_Authlogout = typeof post_Authlogout;
export const post_Authlogout = {
  method: z.literal('POST'),
  path: z.literal('/auth/logout'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: z.unknown(),
};

export type post_Authregister = typeof post_Authregister;
export const post_Authregister = {
  method: z.literal('POST'),
  path: z.literal('/auth/register'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: RegistrationDto,
  }),
  response: z.unknown(),
};

export type post_AuthsendConfirmEmail = typeof post_AuthsendConfirmEmail;
export const post_AuthsendConfirmEmail = {
  method: z.literal('POST'),
  path: z.literal('/auth/send-confirm-email'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: SendConfirmEmailDto,
  }),
  response: z.unknown(),
};

export type get_AuthconfirmEmail = typeof get_AuthconfirmEmail;
export const get_AuthconfirmEmail = {
  method: z.literal('GET'),
  path: z.literal('/auth/confirm-email'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      userId: z.string().optional(),
      token: z.string().optional(),
    }),
  }),
  response: z.unknown(),
};

export type post_AuthchangeLogin = typeof post_AuthchangeLogin;
export const post_AuthchangeLogin = {
  method: z.literal('POST'),
  path: z.literal('/auth/change-login'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: ChangeLoginDto,
  }),
  response: z.unknown(),
};

export type get_AuthconfirmChangeLogin = typeof get_AuthconfirmChangeLogin;
export const get_AuthconfirmChangeLogin = {
  method: z.literal('GET'),
  path: z.literal('/auth/confirm-change-login'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      userId: z.string().optional(),
      login: z.string().optional(),
      token: z.string().optional(),
    }),
  }),
  response: z.unknown(),
};

export type post_AuthchangeEmail = typeof post_AuthchangeEmail;
export const post_AuthchangeEmail = {
  method: z.literal('POST'),
  path: z.literal('/auth/change-email'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: EmailDto,
  }),
  response: z.unknown(),
};

export type get_AuthconfirmChangeEmail = typeof get_AuthconfirmChangeEmail;
export const get_AuthconfirmChangeEmail = {
  method: z.literal('GET'),
  path: z.literal('/auth/confirm-change-email'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      userId: z.string().optional(),
      email: z.string().optional(),
      token: z.string().optional(),
    }),
  }),
  response: z.unknown(),
};

export type post_AuthchangePassword = typeof post_AuthchangePassword;
export const post_AuthchangePassword = {
  method: z.literal('POST'),
  path: z.literal('/auth/change-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: ChangePasswordDto,
  }),
  response: z.unknown(),
};

export type get_AuthconfirmChangePassword = typeof get_AuthconfirmChangePassword;
export const get_AuthconfirmChangePassword = {
  method: z.literal('GET'),
  path: z.literal('/auth/confirm-change-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      userId: z.string().optional(),
      currentPassword: z.string().optional(),
      newPassword: z.string().optional(),
    }),
  }),
  response: z.unknown(),
};

export type post_AuthresetPassword = typeof post_AuthresetPassword;
export const post_AuthresetPassword = {
  method: z.literal('POST'),
  path: z.literal('/auth/reset-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: EmailDto,
  }),
  response: z.unknown(),
};

export type post_AuthconfirmResetPassword = typeof post_AuthconfirmResetPassword;
export const post_AuthconfirmResetPassword = {
  method: z.literal('POST'),
  path: z.literal('/auth/confirm-reset-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: ResetPasswordDto,
  }),
  response: z.unknown(),
};

export type post_Apiv1authforgotPassword = typeof post_Apiv1authforgotPassword;
export const post_Apiv1authforgotPassword = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/auth/forgot-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: ForgotPasswordRequest,
  }),
  response: z.unknown(),
};

export type post_Apiv1authresetPassword = typeof post_Apiv1authresetPassword;
export const post_Apiv1authresetPassword = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/auth/reset-password'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: ResetPasswordRequest,
  }),
  response: z.unknown(),
};

export type get_Apiv1authinfo = typeof get_Apiv1authinfo;
export const get_Apiv1authinfo = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/auth/info'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: InfoResponse,
};

export type post_Apiv1authinfo = typeof post_Apiv1authinfo;
export const post_Apiv1authinfo = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/auth/info'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: InfoRequest,
  }),
  response: InfoResponse,
};

export type get_Debugclaims = typeof get_Debugclaims;
export const get_Debugclaims = {
  method: z.literal('GET'),
  path: z.literal('/debug/claims'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: z.unknown(),
};

export type get_Apiv1filesFileId = typeof get_Apiv1filesFileId;
export const get_Apiv1filesFileId = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/files/{fileId}'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    path: z.object({
      fileId: z.string(),
    }),
  }),
  response: Void,
};

export type get_Apiv1innSearch = typeof get_Apiv1innSearch;
export const get_Apiv1innSearch = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/inn-search'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      query: z.string(),
    }),
  }),
  response: z.array(GetCompanyOut),
};

export type get_GetCurrentUserInfo = typeof get_GetCurrentUserInfo;
export const get_GetCurrentUserInfo = {
  method: z.literal('GET'),
  path: z.literal('/auth/current-user-info'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: CurrentUserInfoDto,
};

export type get_Apiv1companies = typeof get_Apiv1companies;
export const get_Apiv1companies = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/companies'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      status: z.union([z.literal('Init'), z.literal('Approved'), z.literal('Pending')]),
    }),
  }),
  response: z.array(CompanyOut),
};

export type get_Apiv1productId = typeof get_Apiv1productId;
export const get_Apiv1productId = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/product/{id}'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    path: z.object({
      id: z.string(),
    }),
  }),
  response: GetProductOut,
};

export type patch_Apiv1productId = typeof patch_Apiv1productId;
export const patch_Apiv1productId = {
  method: z.literal('PATCH'),
  path: z.literal('/api/v1/product/{id}'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    query: z.object({
      productId: z.string(),
    }),
    body: UpdateProductIn,
  }),
  response: GetProductOut,
};

export type get_Apiv1products = typeof get_Apiv1products;
export const get_Apiv1products = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/products'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: Void,
};

export type get_Apiv1accountproducts = typeof get_Apiv1accountproducts;
export const get_Apiv1accountproducts = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/account/products'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: z.array(GetProductOut),
};

export type post_Apiv1product = typeof post_Apiv1product;
export const post_Apiv1product = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/product'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: AddProductDraftIn,
  }),
  response: GetProductOut,
};

export type get_Apiv1profile = typeof get_Apiv1profile;
export const get_Apiv1profile = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/profile'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: UserProfileOut,
};

export type patch_Apiv1profile = typeof patch_Apiv1profile;
export const patch_Apiv1profile = {
  method: z.literal('PATCH'),
  path: z.literal('/api/v1/profile'),
  requestFormat: z.literal('json'),
  parameters: z.object({
    body: UpdateProfileIn,
  }),
  response: UpdateProfileOut,
};

export type get_Apiv1profileoptions = typeof get_Apiv1profileoptions;
export const get_Apiv1profileoptions = {
  method: z.literal('GET'),
  path: z.literal('/api/v1/profile/options'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: ProfileOptionsOut,
};

export type post_Apiv1profileavatar = typeof post_Apiv1profileavatar;
export const post_Apiv1profileavatar = {
  method: z.literal('POST'),
  path: z.literal('/api/v1/profile/avatar'),
  requestFormat: z.literal('form-data'),
  parameters: z.object({
    body: z.object({
      imageFile: z.string(),
    }),
  }),
  response: z.unknown(),
};

export type delete_Apiv1profileavatar = typeof delete_Apiv1profileavatar;
export const delete_Apiv1profileavatar = {
  method: z.literal('DELETE'),
  path: z.literal('/api/v1/profile/avatar'),
  requestFormat: z.literal('json'),
  parameters: z.never(),
  response: z.unknown(),
};

// <EndpointByMethod>
export const EndpointByMethod = {
  get: {
    '/api/v1/account': get_Apiv1account,
    '/api/v1/account/options': get_Apiv1accountoptions,
    '/api/v1/account/invitation/invite-accept': get_Apiv1accountinvitationinviteAccept,
    '/auth/confirm-email': get_AuthconfirmEmail,
    '/auth/confirm-change-login': get_AuthconfirmChangeLogin,
    '/auth/confirm-change-email': get_AuthconfirmChangeEmail,
    '/auth/confirm-change-password': get_AuthconfirmChangePassword,
    '/api/v1/auth/info': get_Apiv1authinfo,
    '/debug/claims': get_Debugclaims,
    '/api/v1/files/{fileId}': get_Apiv1filesFileId,
    '/api/v1/inn-search': get_Apiv1innSearch,
    '/auth/current-user-info': get_GetCurrentUserInfo,
    '/api/v1/companies': get_Apiv1companies,
    '/api/v1/product/{id}': get_Apiv1productId,
    '/api/v1/products': get_Apiv1products,
    '/api/v1/account/products': get_Apiv1accountproducts,
    '/api/v1/profile': get_Apiv1profile,
    '/api/v1/profile/options': get_Apiv1profileoptions,
  },
  post: {
    '/api/v1/account': post_Apiv1account,
    '/api/v1/brand/avatar': post_Apiv1brandavatar,
    '/api/v1/account/invitation/invite': post_Apiv1accountinvitationinvite,
    '/auth/login': post_Authlogin,
    '/auth/logout': post_Authlogout,
    '/auth/register': post_Authregister,
    '/auth/send-confirm-email': post_AuthsendConfirmEmail,
    '/auth/change-login': post_AuthchangeLogin,
    '/auth/change-email': post_AuthchangeEmail,
    '/auth/change-password': post_AuthchangePassword,
    '/auth/reset-password': post_AuthresetPassword,
    '/auth/confirm-reset-password': post_AuthconfirmResetPassword,
    '/api/v1/auth/forgot-password': post_Apiv1authforgotPassword,
    '/api/v1/auth/reset-password': post_Apiv1authresetPassword,
    '/api/v1/auth/info': post_Apiv1authinfo,
    '/api/v1/product': post_Apiv1product,
    '/api/v1/profile/avatar': post_Apiv1profileavatar,
  },
  put: {
    '/api/v1/account/{accountId}': put_Apiv1accountAccountId,
    '/api/v1/active-account': put_Apiv1activeAccount,
    '/api/v1/account/invitation/update-roles': put_Apiv1accountinvitationupdateRoles,
  },
  patch: {
    '/api/v1/account/{accountId}/developer': patch_Apiv1accountAccountIddeveloper,
    '/api/v1/brand': patch_Apiv1brand,
    '/api/v1/product/{id}': patch_Apiv1productId,
    '/api/v1/profile': patch_Apiv1profile,
  },
  delete: {
    '/api/v1/brand/avatar': delete_Apiv1brandavatar,
    '/api/v1/account/invitation/remove': delete_Apiv1accountinvitationremove,
    '/api/v1/profile/avatar': delete_Apiv1profileavatar,
  },
};
export type EndpointByMethod = typeof EndpointByMethod;
// </EndpointByMethod>

// <EndpointByMethod.Shorthands>
export type GetEndpoints = EndpointByMethod['get'];
export type PostEndpoints = EndpointByMethod['post'];
export type PutEndpoints = EndpointByMethod['put'];
export type PatchEndpoints = EndpointByMethod['patch'];
export type DeleteEndpoints = EndpointByMethod['delete'];
export type AllEndpoints = EndpointByMethod[keyof EndpointByMethod];
// </EndpointByMethod.Shorthands>

// <ApiClientTypes>
export type EndpointParameters = {
  body?: unknown;
  query?: Record<string, unknown>;
  header?: Record<string, unknown>;
  path?: Record<string, unknown>;
};

export type MutationMethod = 'post' | 'put' | 'patch' | 'delete';
export type Method = 'get' | 'head' | 'options' | MutationMethod;

type RequestFormat = 'json' | 'form-data' | 'form-url' | 'binary' | 'text';

export type DefaultEndpoint = {
  parameters?: EndpointParameters | undefined;
  response: unknown;
};

export type Endpoint<TConfig extends DefaultEndpoint = DefaultEndpoint> = {
  operationId: string;
  method: Method;
  path: string;
  requestFormat: RequestFormat;
  parameters?: TConfig['parameters'];
  meta: {
    alias: string;
    hasParameters: boolean;
    areParametersRequired: boolean;
  };
  response: TConfig['response'];
};

type Fetcher = (
  method: Method,
  url: string,
  parameters?: EndpointParameters | undefined,
) => Promise<Endpoint['response']>;

type RequiredKeys<T> = {
  [P in keyof T]-?: undefined extends T[P] ? never : P;
}[keyof T];

type MaybeOptionalArg<T> = RequiredKeys<T> extends never ? [config?: T] : [config: T];

// </ApiClientTypes>

// <ApiClient>
export class ApiClient {
  baseUrl: string = '';

  constructor(public fetcher: Fetcher) {}

  setBaseUrl(baseUrl: string) {
    this.baseUrl = baseUrl;
    return this;
  }

  // <ApiClient.get>
  get<Path extends keyof GetEndpoints, TEndpoint extends GetEndpoints[Path]>(
    path: Path,
    ...params: MaybeOptionalArg<z.infer<TEndpoint['parameters']>>
  ): Promise<z.infer<TEndpoint['response']>> {
    return this.fetcher('get', this.baseUrl + path, params[0]) as Promise<z.infer<TEndpoint['response']>>;
  }
  // </ApiClient.get>

  // <ApiClient.post>
  post<Path extends keyof PostEndpoints, TEndpoint extends PostEndpoints[Path]>(
    path: Path,
    ...params: MaybeOptionalArg<z.infer<TEndpoint['parameters']>>
  ): Promise<z.infer<TEndpoint['response']>> {
    return this.fetcher('post', this.baseUrl + path, params[0]) as Promise<z.infer<TEndpoint['response']>>;
  }
  // </ApiClient.post>

  // <ApiClient.put>
  put<Path extends keyof PutEndpoints, TEndpoint extends PutEndpoints[Path]>(
    path: Path,
    ...params: MaybeOptionalArg<z.infer<TEndpoint['parameters']>>
  ): Promise<z.infer<TEndpoint['response']>> {
    return this.fetcher('put', this.baseUrl + path, params[0]) as Promise<z.infer<TEndpoint['response']>>;
  }
  // </ApiClient.put>

  // <ApiClient.patch>
  patch<Path extends keyof PatchEndpoints, TEndpoint extends PatchEndpoints[Path]>(
    path: Path,
    ...params: MaybeOptionalArg<z.infer<TEndpoint['parameters']>>
  ): Promise<z.infer<TEndpoint['response']>> {
    return this.fetcher('patch', this.baseUrl + path, params[0]) as Promise<z.infer<TEndpoint['response']>>;
  }
  // </ApiClient.patch>

  // <ApiClient.delete>
  delete<Path extends keyof DeleteEndpoints, TEndpoint extends DeleteEndpoints[Path]>(
    path: Path,
    ...params: MaybeOptionalArg<z.infer<TEndpoint['parameters']>>
  ): Promise<z.infer<TEndpoint['response']>> {
    return this.fetcher('delete', this.baseUrl + path, params[0]) as Promise<z.infer<TEndpoint['response']>>;
  }
  // </ApiClient.delete>
}

export function createApiClient(fetcher: Fetcher, baseUrl?: string) {
  return new ApiClient(fetcher).setBaseUrl(baseUrl ?? '');
}

/**
 Example usage:
 const api = createApiClient((method, url, params) =>
   fetch(url, { method, body: JSON.stringify(params) }).then((res) => res.json()),
 );
 api.get("/users").then((users) => console.log(users));
 api.post("/users", { body: { name: "John" } }).then((user) => console.log(user));
 api.put("/users/:id", { path: { id: 1 }, body: { name: "John" } }).then((user) => console.log(user));
*/

// </ApiClient
