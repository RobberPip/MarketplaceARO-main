import { Button } from 'primereact/button';
import { Checkbox } from 'primereact/checkbox';
import { Divider } from 'primereact/divider';
import { InputText } from 'primereact/inputtext';
import { InputTextarea } from 'primereact/inputtextarea';
import { BrandAccount } from './brand';
import { useUnit } from 'effector-react';
import {
  $inviteModal,
  $suggestions,
  accountOptionsQuery,
  accountQuery,
  AccountRole,
  addAccountMutation,
  addPublicAccountForm,
  closeInviteModal,
  deleteUserMutation,
  fetchSuggestionsRaw,
  inviteUserForm,
  inviteUserMutation,
  mapInvitations,
  openInviteModal,
  ROLE_MAP,
  STATUS_MAP,
  updateRoleUsersMutation,
} from '../model';
import { AutoComplete, AutoCompleteChangeEvent } from 'primereact/autocomplete';
import { components } from '~/shared/api/schema';
import { itemTemplate } from '..';
import { useRef, useState } from 'react';
import { Dialog } from 'primereact/dialog';
import { DataTable } from 'primereact/datatable';
import { Column } from 'primereact/column';
import { Avatar } from 'primereact/avatar';
import { Message } from 'primereact/message';
import { Dropdown } from 'primereact/dropdown';
import { Menu } from 'primereact/menu';
import { Tag } from 'primereact/tag';
import { DeleteAccount } from './delete';
import { DeveloperAccount } from './developer';
import { number } from 'zod';

export function InvitionUsers({ accountData }: { accountData: any }) {
  const inviteModal = useUnit($inviteModal);
  const openModal = useUnit(openInviteModal);
  const closeModal = useUnit(closeInviteModal);
  const { pending: pendingInvite } = useUnit(inviteUserMutation);
  const { extendedForm: formInvite } = useUnit(inviteUserForm);
  const mappedUsers = mapInvitations(accountData.invitations);
  const [hoveredRowIndex, setHoveredRowIndex] = useState<number | null>();
  const menuRef = useRef<Menu>(null);
  const selectedRowDataRef = useRef<any>(null);
  const [editRowKeys, setEditRowKeys] = useState<string[]>([]);
  const [editedRoles, setEditedRoles] = useState<Record<string, string>>({});
  const roles = Object.entries(ROLE_MAP)
    .filter(([key]) => key !== 'Owner')
    .map(([key, label]) => ({ label, value: key }));
  return (
    <div hidden={!accountData.id}>
      <div className="flex justify-content-between">
        <h2 className="m-0 text-lg font-medium">Пользователи аккаунта</h2>
        {accountData.userRole === 'Owner' && (
          <Button label="Пригласить" severity="secondary" onClick={() => openModal()} />
        )}

        <Dialog
          header="Приглашение пользователя"
          className="text-sm"
          headerClassName="pb-3"
          visible={inviteModal}
          style={{ width: '512px' }}
          onHide={() => {
            closeModal();
            formInvite.reset();
          }}
        >
          <form
            onSubmit={(e) => {
              e.preventDefault();
              formInvite.handleSubmit();
            }}
          >
            <span>
              Пригласите пользователя и выберите ему роль в вашем
              <br /> аккаунте
            </span>
            <formInvite.Field
              name="email"
              children={(field) => (
                <label className="flex mt-3 gap-1 w-21rem text-xs flex-column text-color-secondary">
                  Почта
                  <InputText
                    className=" mt-1"
                    placeholder="Введите"
                    id={field.name}
                    name={field.name}
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(e) => field.handleChange(e.target.value)}
                    invalid={field.state.meta.errors.length > 0}
                  />
                  {field.state.meta.errors.length > 0 && (
                    <Message className="w-full mt-1" severity="error" text={field.state.meta.errors.join('\n')} />
                  )}
                </label>
              )}
            />
            <formInvite.Field
              name="role"
              children={(field) => (
                <label className="flex mt-3 gap-1 w-21rem text-xs flex-column text-color-secondary">
                  Роль
                  <Dropdown
                    className="mt-1"
                    id={field.name}
                    name={field.name}
                    value={field.state.value}
                    options={roles}
                    onChange={(e) => field.handleChange(e.value)}
                    onBlur={field.handleBlur}
                    placeholder="Выберите роль"
                  />
                </label>
              )}
            />
            <div className="flex justify-content-between mb-4 mt-4">
              <formInvite.Subscribe
                selector={(state) => state.isTouched && state.canSubmit}
                children={(canSubmit) => (
                  <>
                    <Button
                      label="Отменить"
                      severity="secondary"
                      type="reset"
                      onClick={() => {
                        closeModal();
                        formInvite.reset();
                      }}
                      className="w-13rem"
                    />
                    <Button
                      label="Пригласить"
                      loading={pendingInvite}
                      disabled={!canSubmit}
                      className="w-13rem"
                      type="submit"
                    />
                  </>
                )}
              />
            </div>
          </form>
        </Dialog>
      </div>

      <Divider className="mt-2 mb-2" />
      <DataTable
        value={mappedUsers}
        onRowMouseEnter={(e) => setHoveredRowIndex(e.index)}
        onRowMouseLeave={() => setHoveredRowIndex(null)}
        rowClassName={() => 'hover:surface-100'}
      >
        <Column
          field="user"
          header="Пользователь"
          className="w-25rem"
          body={(row) => (
            <div className="flex">
              <Avatar
                image={row.avatar}
                className="mr-2 mt-1 border-1 bg-white"
                shape="circle"
                label={
                  !row.avatar
                    ? (row.res.profile?.lastName?.[0]?.toUpperCase() ?? '') +
                      (row.res.profile?.firstName?.[0]?.toUpperCase() ?? '')
                    : undefined
                }
              />
              <div className="flex flex-column">
                <span>{row.user}</span>
                <span className="text-color-secondary">{row.email}</span>
              </div>
            </div>
          )}
        />
        <Column
          field="role"
          header="Роль"
          className="w-10rem"
          body={(rowData) =>
            editRowKeys.includes(rowData.id) ? (
              <Dropdown
                value={editedRoles[rowData.id] ?? rowData.roleKey}
                options={roles}
                onChange={(e) =>
                  setEditedRoles((prev) => ({
                    ...prev,
                    [rowData.id]: e.value,
                  }))
                }
                className="w-9rem"
              />
            ) : (
              <span>{rowData.roleLabel}</span>
            )
          }
        />
        <Column
          field="status"
          header="Статус"
          body={(row) => {
            const statusInfo = STATUS_MAP[row.status] ?? { label: row.status, severity: 'info' };
            return (
              <div className="flex align-items-center">
                <Tag
                  severity={statusInfo.severity}
                  className="border-circle p-0 mr-2"
                  style={{
                    width: '6px',
                    height: '6px',
                  }}
                />
                <span>{statusInfo.label}</span>
              </div>
            );
          }}
        />

        {accountData.userRole === 'Owner' && (
          <Column
            header=""
            className="w-2rem text-center"
            body={(rowData, { rowIndex }) =>
              rowData.roleKey !== 'Owner' ? (
                <span
                  className="icon-2xl icon-dots-vertical cursor-pointer"
                  onClick={(event) => {
                    selectedRowDataRef.current = rowData;
                    menuRef.current?.toggle(event);
                  }}
                />
              ) : null
            }
          />
        )}
      </DataTable>

      <Menu
        ref={menuRef}
        popup
        className="w-9rem text-color-secondary border-round-xl border-1"
        model={[
          {
            label: 'Редактировать',
            command: () => {
              if (!selectedRowDataRef.current) return;
              const id = selectedRowDataRef.current.id;
              setEditRowKeys((prev) => (prev.includes(id) ? prev : [...prev, id]));
              setEditedRoles((prev) => ({
                ...prev,
                [id]: selectedRowDataRef.current!.roleKey,
              }));
            },
          },
          {
            label: 'Удалить',
            template: (item) => (
              <div
                onClick={(e) => {
                  if (!selectedRowDataRef.current) return;
                  menuRef.current?.hide(e);
                  deleteUserMutation.start({ invitationId: selectedRowDataRef.current.id });
                }}
                className="text-red-600 p-2 pl-3 my-0 py-1 p-menuitem-exit"
              >
                {item.label}
              </div>
            ),
          },
        ]}
      />
      {editRowKeys.length > 0 && (
        <Button
          type="button"
          label="Сохранить"
          className="my-4"
          onClick={() => {
            const payload = editRowKeys
              .map((id) => {
                const role = editedRoles[id];
                if (!role) return undefined;
                return {
                  accountProfileId: id,
                  role: role as AccountRole,
                };
              })
              .filter((item): item is { accountProfileId: string; role: AccountRole } => Boolean(item));

            if (payload.length > 0) {
              updateRoleUsersMutation.start(payload);
              setEditRowKeys([]);
              setEditedRoles({});
            }
          }}
        />
      )}
    </div>
  );
}
