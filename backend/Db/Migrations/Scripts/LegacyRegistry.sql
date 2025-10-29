create sequence guide_attribute_id_seq
    as integer;

alter sequence guide_attribute_id_seq owner to postgres;

create sequence possible_values_id_seq
    as integer;

alter sequence possible_values_id_seq owner to postgres;

create sequence guide_scope_attribute_seq
    as integer;

alter sequence guide_scope_attribute_seq owner to postgres;

create sequence software_id_seq;

alter sequence software_id_seq owner to postgres;

create sequence testid_seq
    as integer;

alter sequence testid_seq owner to postgres;

create sequence user_id_seq
    as integer;

alter sequence user_id_seq owner to postgres;

create sequence vendor_id_seq;

alter sequence vendor_id_seq owner to postgres;

create table guide_scope_attribute
(
    id    integer default nextval('guide_scope_attribute_seq'::regclass) not null
        constraint guide_scope_attribute_pk
            primary key,
    scope varchar
);

alter table guide_scope_attribute
    owner to postgres;

create table guide_attribute
(
    id                   bigint  default nextval('guide_attribute_id_seq'::regclass) not null
        constraint guide_attribute_pk
            primary key,
    attribute_name       varchar
        constraint unique_attribute_name
            unique,
    filterable_attribute boolean default true                                                          not null,
    id_scope             integer default 1
        constraint guide_attribute_guide_scope_attribute_id_fk
            references guide_scope_attribute
);

alter table guide_attribute
    owner to postgres;

alter sequence possible_values_id_seq owned by guide_attribute.id;

create table attribute_possible_values
(
    id           integer default nextval('possible_values_id_seq'::regclass) not null
        constraint possible_values_pkey
            primary key,
    attribute_id integer
        constraint attribure_possible_values_guide_attribute_id_fk
            references guide_attribute,
    value        character varying[]
);

alter table attribute_possible_values
    owner to postgres;

-- create table programmy
-- (
--     work_with_bim                 text,
--     basic_software                text,
--     plagin                        text,
--     name                          text,
--     description                   text,
--     web_link                      text,
--     stage_base                    text,
--     type_base                     text,
--     activity_base                 text,
--     stage                         text,
--     type                          text,
--     activity                      text,
--     rr_name                       text,
--     rr_reg_number                 integer,
--     rr_class                      text,
--     source_name                   text,
--     rr_name_copyright_holder_base text,
--     analogs                       text,
--     verify                        text,
--     rr_name_copyright_holder      text,
--     process                       text,
--     to_delete                     text,
--     id                            serial,
--     notim                         varchar(3) default 'No'::character varying
-- );

-- alter table programmy
--     owner to postgres;

-- create table programmy2
-- (
--     work_with_bim                 text,
--     basic_software                text,
--     plagin                        text,
--     name                          text,
--     description                   text,
--     web_link                      text,
--     stage_base                    text,
--     type_base                     text,
--     activity_base                 text,
--     stage                         text,
--     type                          text,
--     activity                      text,
--     rr_name                       text,
--     rr_reg_number                 integer,
--     rr_class                      text,
--     source_name                   text,
--     rr_name_copyright_holder_base text,
--     analogs                       text,
--     verify                        text,
--     rr_name_copyright_holder      text,
--     process                       text,
--     to_delete                     text
-- );

-- alter table programmy2
--     owner to postgres;

-- create table role
-- (
--     id        integer not null
--         constraint role_pk
--             primary key,
--     role_name text    not null
-- );

-- alter table role
--     owner to postgres;

create table software
(
    id          integer   default nextval('software_id_seq'::regclass) not null
        constraint software_pk
            primary key,
    name        varchar                                                                  not null,
    description varchar,
    link_to     varchar,
    logo        bytea,
    date_to_add timestamp default CURRENT_TIMESTAMP
);

comment on column software.name is 'Наименование';

comment on column software.description is 'Описание';

comment on column software.link_to is 'Ссылка на сайт ПО';

comment on column software.logo is 'Логотип ПО';

alter table software
    owner to postgres;

alter sequence software_id_seq owned by software.id;

-- create table users
-- (
--     id       bigint default nextval('user_id_seq'::regclass) not null
--         constraint users_pk
--             primary key,
--     name     text                                                              not null,
--     email    text,
--     password text,
--     role_id  integer                                                           not null
-- );

-- alter table users
--     owner to postgres;

-- alter sequence user_id_seq owned by users.id;

create table vendors
(
    id          bigint    default nextval('vendor_id_seq'::regclass) not null
        constraint vendors_pk
            primary key,
    name        varchar                                                                not null,
    description varchar,
    link_to     varchar,
    other_name  varchar,
    date_to_add timestamp default CURRENT_TIMESTAMP,
    public_name varchar,
    logo        bytea
);

alter table vendors
    owner to postgres;

alter sequence vendor_id_seq owned by vendors.id;

create table software_vendors_links
(
    software_id integer not null
        constraint software_vendors_software_id_fkey
            references software,
    vendor_id   integer not null
        constraint software_vendors_vendor_id_fkey
            references vendors,
    constraint software_vendors_pkey
        primary key (software_id, vendor_id)
);

alter table software_vendors_links
    owner to postgres;

create table value_attribute
(
    id           serial
        constraint value_attribute_pk
            primary key,
    software_id  integer
        constraint value_attribute_software_id_fk
            references software,
    attribute_id integer
        constraint value_attribute_guide_attribute_id_fk
            references guide_attribute,
    value        character varying[],
    type_data    varchar,
    vendor_id    integer
        constraint value_attribute_vendors_id_fk
            references vendors,
    _value       character varying[]
);

alter table value_attribute
    owner to postgres;

-- create table vendory
-- (
--     c1    text,
--     c2    text,
--     c3    text,
--     c4    text,
--     c5    text,
--     c6    text,
--     c7    text,
--     notim text,
--     c9    text,
--     c10   text,
--     c11   text,
--     id    serial
-- );

-- alter table vendory
--     owner to postgres;

create table filtershortlinks
(
    id             serial
        primary key,
    unique_numcode integer   not null,
    hash_json_data varchar   not null,
    source_json    json      not null,
    add_date       timestamp not null
);

alter table filtershortlinks
    owner to postgres;

create view v_value_attribute (id, software_id, attribute_id, attribute_name, value, type_data, vendor_id, _value) as
SELECT value_attribute.id,
       value_attribute.software_id,
       value_attribute.attribute_id,
       ga.attribute_name,
       value_attribute.value,
       value_attribute.type_data,
       value_attribute.vendor_id,
       value_attribute._value
FROM value_attribute
         JOIN guide_attribute ga ON ga.id = value_attribute.attribute_id;

alter table v_value_attribute
    owner to postgres;

create procedure "AddAttribute"(IN _name text, IN _filterable boolean, IN _scope text)
    language plpgsql
as
$$
DECLARE
    _id bigint;
BEGIN
    INSERT INTO guide_attribute (attribute_name, filterable_attribute,id_scope)
    VALUES (_name, _filterable,(SELECT id FROM guide_scope_attribute WHERE scope = _scope));
END;
$$;

alter procedure "AddAttribute"(text, boolean, text) owner to postgres;

create procedure "AddSoftware"(IN _name text, IN _logo bytea DEFAULT NULL::bytea, IN _description text DEFAULT NULL::text, IN _link_to text DEFAULT NULL::text, IN keys text[] DEFAULT NULL::text[], IN data_values text[] DEFAULT NULL::text[], IN _vendor text DEFAULT NULL::text, IN data_type text[] DEFAULT NULL::text[], IN fkeys text[] DEFAULT NULL::text[], IN fdata_values text[] DEFAULT NULL::text[])
    language plpgsql
as
$$
DECLARE
    _id BIGINT;
    _id_attribute_name BIGINT;
    _temp_boolean BOOLEAN;
    _temp_values TEXT[];
    vendor_name TEXT;

    -- Изменения: объявление новой переменной для преобразования строки в массив
    _data_values_array text[];
BEGIN
    -- Вставляем новые данные в software
    INSERT INTO software (
        name,
        logo,
        description,
        link_to
    ) VALUES (
                 _name,
                 _logo,
                 _description,
                 _link_to
             ) RETURNING id INTO _id;

    -- Проверяем и вставляем связь между поставщиком и программным обеспечением в таблицу software_vendors_links
    -- Вставляем связи между поставщиками и программным обеспечением в таблицу software_vendors_links
    IF _vendor IS NOT NULL THEN
        -- Разделяем строку _vendor на отдельные вендоры, используя запятую в качестве разделителя
        FOR vendor_name IN SELECT unnest(string_to_array(_vendor, ', ')) LOOP
                -- Для каждого вендора выполняем вставку связи
                INSERT INTO software_vendors_links (software_id, vendor_id)
                VALUES (_id, (SELECT id FROM vendors WHERE name = vendor_name));
            END LOOP;
    END IF;

    -- Вставка атрибутов
    IF keys IS NOT NULL AND data_values IS NOT NULL THEN
        FOR i IN array_lower(keys, 1) .. array_upper(keys, 1) LOOP
  
                _data_values_array := string_to_array(data_values[i], ',,');

                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, id_scope)
                VALUES (keys[i], (SELECT id FROM guide_scope_attribute WHERE scope = 'Software'))
                ON CONFLICT (attribute_name) DO UPDATE
                    SET id_scope = CASE
                                       WHEN guide_attribute.id_scope <> (SELECT id FROM guide_scope_attribute WHERE scope = 'Software')
                                           THEN (SELECT id FROM guide_scope_attribute WHERE scope = 'Global')
                                       ELSE guide_attribute.id_scope
                        END
                RETURNING id INTO _id_attribute_name;

                -- Получение ID атрибута, независимо от того, был ли он только что создан или уже существовал
                SELECT id INTO _id_attribute_name FROM guide_attribute WHERE attribute_name = keys[i];

                -- Вставка данных в value_attribute
                INSERT INTO value_attribute (attribute_id, value, software_id, type_data)
                VALUES (_id_attribute_name, _data_values_array, _id, data_type[i]);
            END LOOP;
    END IF;

    IF fkeys IS NOT NULL AND fdata_values IS NOT NULL THEN
        FOR i IN array_lower(fkeys, 1) .. array_upper(fkeys, 1) LOOP
                -- Преобразование текстового значения в булево
                _temp_boolean := CASE
                                     WHEN LOWER(fdata_values[i]) = 'true' OR fdata_values[i] = '1' THEN true
                                     ELSE false
                    END;

                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, filterable_attribute)
                VALUES (fkeys[i], _temp_boolean)
                ON CONFLICT (attribute_name) DO UPDATE
                    SET filterable_attribute = EXCLUDED.filterable_attribute
                RETURNING id INTO _id_attribute_name;
            END LOOP;
    END IF;
END
$$;

alter procedure "AddSoftware"(text, bytea, text, text, text[], text[], text, text[], text[], text[]) owner to postgres;

create procedure "DeleteAttributeById"(IN _id bigint)
    language plpgsql
as
$$
BEGIN
    -- Проверяем, есть ли зависимые значения в value_attribute
    IF NOT EXISTS (SELECT 1 FROM "value_attribute" WHERE "attribute_id" = _id) THEN
        -- Если нет зависимых значений, то удаляем запись из guide_attribute
        DELETE FROM "guide_attribute" WHERE "id" = _id;
    END IF;
END;
$$;

alter procedure "DeleteAttributeById"(bigint) owner to postgres;

create procedure "DeleteSoftwareById"(IN _id bigint)
    language plpgsql
as
$$
BEGIN
    DELETE FROM value_attribute WHERE software_id = _id;
    DELETE FROM software_vendors_links WHERE software_id = _id;
    DELETE FROM software WHERE id = _id;
END
$$;

alter procedure "DeleteSoftwareById"(bigint) owner to postgres;

create procedure "DeleteVendorById"(IN _id bigint)
    language plpgsql
as
$$
BEGIN
    DELETE FROM value_attribute WHERE vendor_id = _id;
    DELETE FROM software_vendors_links WHERE vendor_id = _id;
    DELETE FROM vendors WHERE Id = _id;
END;
$$;

alter procedure "DeleteVendorById"(bigint) owner to postgres;

create procedure "UpdateAttribute"(IN _id bigint, IN _name text, IN _filterable boolean, IN _scope text)
    language plpgsql
as
$$
    DECLARE
     scope_id integer;
BEGIN
    SELECT id INTO scope_id FROM guide_scope_attribute WHERE scope ilike _scope;
    IF (scope_id IS NULL)
        THEN
            RAISE EXCEPTION 'Operation is not possible, this scope does not exist';
    ELSE
        UPDATE guide_attribute SET attribute_name = _name, filterable_attribute= _filterable, id_scope = scope_id WHERE id = _id;
    END IF;
END
$$;

alter procedure "UpdateAttribute"(bigint, text, boolean, text) owner to postgres;

create procedure "UpdateSoftware"(IN _id bigint, IN _flag_logo boolean, IN _flag_vendor boolean, IN _flag_link_to boolean, IN _flag_description boolean, IN _name text DEFAULT NULL::text, IN _description text DEFAULT NULL::text, IN _link_to text DEFAULT NULL::text, IN _logo bytea DEFAULT NULL::bytea, IN _vendor text DEFAULT NULL::text, IN keys text[] DEFAULT NULL::text[], IN data_values text[] DEFAULT NULL::text[], IN data_type text[] DEFAULT NULL::text[], IN fkeys text[] DEFAULT NULL::text[], IN fdata_values text[] DEFAULT NULL::text[])
    language plpgsql
as
$$
DECLARE
    _id_attribute_name bigint;
    _temp_values text[];
    _id_attribute_value bigint;
    _temp_boolean BOOLEAN;
    vendor_name text;
    _invalid_vendors text[] := ARRAY[]::text[];
BEGIN
    -- Обновление имени
    IF _name IS NOT NULL THEN
        UPDATE software SET name = _name WHERE id = _id;
    END IF;

    -- Обновление описания
    IF _flag_description THEN
        UPDATE software SET description = _description WHERE id = _id;
    END IF;

    -- Обновление ссылки
    IF _flag_link_to THEN
        UPDATE software SET link_to = _link_to WHERE id = _id;
    END IF;

    -- Обновление логотипа
    IF _flag_logo THEN
        UPDATE software SET logo = _logo WHERE id = _id;
    END IF;

    -- Обновление вендора
    IF _flag_vendor THEN
        IF _vendor IS NOT NULL THEN
            -- Удаление существующих связей
            DELETE FROM software_vendors_links WHERE software_id = _id;

            -- Разделяем строку _vendor на отдельные вендоры, используя запятую в качестве разделителя
            FOR vendor_name IN SELECT unnest(string_to_array(_vendor, ', ')) LOOP
                    -- Проверка наличия вендора в таблице vendors
                    IF NOT EXISTS (SELECT 1 FROM vendors WHERE name = vendor_name) THEN
                        -- Добавление вендора в массив недопустимых вендоров
                        _invalid_vendors := array_append(_invalid_vendors, vendor_name);
                    ELSE
                        -- Для каждого вендора выполняем вставку связи
                        INSERT INTO software_vendors_links (software_id, vendor_id)
                        VALUES (_id, (SELECT id FROM vendors WHERE name = vendor_name));
                    END IF;
                END LOOP;
        ELSE
            -- Удаление всех существующих связей, если _vendor равен NULL
            DELETE FROM software_vendors_links WHERE software_id = _id;
        END IF;
    END IF;

    -- Проверка наличия недопустимых вендоров и выдача сообщения об ошибках, но не вызывать исключение
    IF array_length(_invalid_vendors, 1) > 0 THEN
        RAISE NOTICE 'The following vendors were not found and were not added: %', array_to_string(_invalid_vendors, ', ');
    END IF;




    -- Обновление атрибутов
    IF keys IS NOT NULL AND data_values IS NOT NULL THEN
        IF array_length(keys, 1) != array_length(data_values, 1) THEN
            RAISE EXCEPTION 'Arrays keys and values must have the same length';
        END IF;

        FOR i IN array_lower(keys, 1) .. array_upper(keys, 1)
            LOOP
                _temp_values := string_to_array(data_values[i], ',,');
                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, id_scope)
                VALUES (keys[i], (SELECT id FROM guide_scope_attribute WHERE scope = 'Software'))
                ON CONFLICT (attribute_name) DO UPDATE
                    SET id_scope = CASE
                                       WHEN guide_attribute.id_scope <> (SELECT id FROM guide_scope_attribute WHERE scope = 'Software')
                                           THEN (SELECT id FROM guide_scope_attribute WHERE scope = 'Global')
                                       ELSE guide_attribute.id_scope
                        END
                RETURNING id INTO _id_attribute_name;



                -- Получение ID атрибута, независимо от того, был ли он только что создан или уже существовал
                SELECT id INTO _id_attribute_name FROM guide_attribute WHERE attribute_name = keys[i];


                IF data_values[i] IS NULL OR data_values[i] = 'null' THEN
                    -- Удалить запись из value_attribute, если data_values[i] равно NULL
                    DELETE FROM value_attribute
                    WHERE attribute_id = _id_attribute_name AND software_id = _id;
                ELSE
                    -- Проверить наличие записи в value_attribute для данного атрибута и программы
                    SELECT id INTO _id_attribute_value
                    FROM value_attribute
                    WHERE attribute_id = _id_attribute_name AND software_id = _id;

                    IF _id_attribute_value IS NULL THEN
                        -- Если записи нет, то добавить ее
                        INSERT INTO value_attribute (attribute_id, value, software_id, type_data)
                        VALUES (_id_attribute_name,_temp_values, _id, data_type[i]);
                    ELSE
                        -- В противном случае обновить данные в value_attribute
                        UPDATE value_attribute
                        SET value = _temp_values, type_data = data_type[i]
                        WHERE id = _id_attribute_value;
                    END IF;
                END IF;
            END LOOP;
    END IF;
    IF fkeys IS NOT NULL AND fdata_values IS NOT NULL THEN
        FOR i IN array_lower(fkeys, 1) .. array_upper(fkeys, 1) LOOP
                -- Преобразование текстового значения в булево
                _temp_boolean := CASE
                                     WHEN LOWER(fdata_values[i]) = 'true' OR fdata_values[i] = '1' THEN true
                                     ELSE false
                    END;

                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, filterable_attribute)
                VALUES (fkeys[i], _temp_boolean)
                ON CONFLICT (attribute_name) DO UPDATE
                    SET filterable_attribute = EXCLUDED.filterable_attribute
                RETURNING id INTO _id_attribute_name;
            END LOOP;
    END IF;
END
$$;

alter procedure "UpdateSoftware"(bigint, boolean, boolean, boolean, boolean, text, text, text, bytea, text, text[], text[], text[], text[], text[]) owner to postgres;

-- create procedure "UpdateUser"(IN _flag_email boolean, IN _flag_password boolean, IN _id bigint, IN _role integer DEFAULT NULL::integer, IN _name text DEFAULT NULL::text, IN _email text DEFAULT NULL::text, IN _password text DEFAULT NULL::text)
--     language plpgsql
-- as
-- $$
-- BEGIN
--     IF _name IS NOT NULL THEN
--         UPDATE users SET name = _name WHERE id = _id;
--     END IF;

--     IF _role IS NOT NULL THEN
--         UPDATE users SET role_id = _role WHERE id = _id;
--     END IF;

--     IF _flag_email IS TRUE THEN
--         UPDATE users SET email = _email WHERE id = _id;
--     END IF;

--     IF _flag_password IS TRUE THEN
--         UPDATE users SET password = _password WHERE id = _id;
--     END IF;
-- END;
-- $$;

-- alter procedure "UpdateUser"(boolean, boolean, bigint, integer, text, text, text) owner to postgres;

create function check_value_attribute_value() returns trigger
    language plpgsql
as
$$
BEGIN
    -- Проверяем, есть ли данные в таблице "possible_values" для данного атрибута
    IF EXISTS (SELECT 1 FROM attribute_possible_values WHERE attribute_id = NEW.attribute_id) THEN
        DECLARE
            possible_values text[];
            new_values text[];
            problem_value text;
        BEGIN
            -- Извлекаем возможные значения из таблицы "attribute_possible_values" в массив
            SELECT ARRAY_AGG(value) FROM attribute_possible_values WHERE attribute_id = NEW.attribute_id INTO possible_values;

-- Преобразуем поле "value" из NEW в массив
            new_values := NEW.value;

-- Проверяем, что каждое значение из вставляемого массива присутствует в массиве ограничений
            FOR problem_value IN SELECT unnest(new_values)
                LOOP
                    IF problem_value NOT IN (SELECT unnest(possible_values))
                    THEN
                        RAISE EXCEPTION 'Значение в поле "value" (%) не соответствует возможным значениям для данного атрибута', problem_value;
                    END IF;
                END LOOP;


        END;
    END IF;

    RETURN NEW;
END;
$$;

alter function check_value_attribute_value() owner to postgres;

create trigger value_attribute_check_value
    before insert or update
    on value_attribute
    for each row
execute procedure check_value_attribute_value();

create function set_default_id_scope() returns trigger
    language plpgsql
as
$$
BEGIN
    IF NEW.id_scope IS NULL THEN
        SELECT id INTO NEW.id_scope FROM guide_scope_attribute WHERE scope = 'Global';
    END IF;
    RETURN NEW;
END;
$$;

alter function set_default_id_scope() owner to postgres;

create trigger trigger_set_default_id_scope
    before insert
    on guide_attribute
    for each row
execute procedure set_default_id_scope();

create procedure "AddVendor"(IN _name text, IN _description text DEFAULT NULL::text, IN _link_to text DEFAULT NULL::text, IN _other_name text DEFAULT NULL::text, IN _public_name text DEFAULT NULL::text, IN keys text[] DEFAULT NULL::text[], IN data_values text[] DEFAULT NULL::text[], IN data_type text[] DEFAULT NULL::text[], IN fkeys text[] DEFAULT NULL::text[], IN fdata_values text[] DEFAULT NULL::text[], IN _logo bytea DEFAULT NULL::bytea)
    language plpgsql
as
$$
DECLARE
    _id BIGINT;
    _id_attribute_name BIGINT;
    _temp_boolean BOOLEAN;
    _temp_values TEXT[];
BEGIN

    -- Вставка данных в таблицу vendors
    INSERT INTO vendors (name, description, link_to, other_name, public_name,logo)
    VALUES (_name, _description, _link_to, _other_name,_public_name, _logo) RETURNING id INTO _id;
    -- Вставка атрибутов
    IF keys IS NOT NULL AND data_values IS NOT NULL THEN
        FOR i IN array_lower(keys, 1) .. array_upper(keys, 1) LOOP
                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, id_scope)
                VALUES (keys[i], (SELECT id FROM guide_scope_attribute WHERE scope = 'Vendor'))
                ON CONFLICT (attribute_name) DO UPDATE
                    SET id_scope = CASE
                                       WHEN guide_attribute.id_scope <> (SELECT id FROM guide_scope_attribute WHERE scope = 'Vendor')
                                           THEN (SELECT id FROM guide_scope_attribute WHERE scope = 'Global')
                                       ELSE guide_attribute.id_scope
                        END
                RETURNING id INTO _id_attribute_name;

                -- Получение ID атрибута, независимо от того, был ли он только что создан или уже существовал
                SELECT id INTO _id_attribute_name FROM guide_attribute WHERE attribute_name = keys[i];

                -- Преобразование text в character varying
                _temp_values[1] := data_values[i]::character varying;

                -- Вставка данных в value_attribute
                INSERT INTO value_attribute (attribute_id, value, vendor_id, type_data)
                VALUES (_id_attribute_name, _temp_values, _id, data_type[i]);
            END LOOP;
    END IF;
    IF fkeys IS NOT NULL AND fdata_values IS NOT NULL THEN
        FOR i IN array_lower(fkeys, 1) .. array_upper(fkeys, 1) LOOP
                -- Преобразование текстового значения в булево
                _temp_boolean := CASE
                                     WHEN LOWER(fdata_values[i]) = 'true' OR fdata_values[i] = '1' THEN true
                                     ELSE false
                    END;

                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, filterable_attribute)
                VALUES (fkeys[i], _temp_boolean)
                ON CONFLICT (attribute_name) DO UPDATE
                    SET filterable_attribute = EXCLUDED.filterable_attribute
                RETURNING id INTO _id_attribute_name;
            END LOOP;
    END IF;
END;
$$;

alter procedure "AddVendor"(text, text, text, text, text, text[], text[], text[], text[], text[], bytea) owner to postgres;

create procedure "UpdateVendor"(IN _flag_logo boolean, IN _flag_link_to boolean, IN _flag_description boolean, IN _flag_other_name boolean, IN _flag_public_name boolean, IN _id bigint, IN _name text DEFAULT NULL::text, IN _description text DEFAULT NULL::text, IN _link_to text DEFAULT NULL::text, IN _other_name text DEFAULT NULL::text, IN _public_name text DEFAULT NULL::text, IN keys text[] DEFAULT NULL::text[], IN data_values text[] DEFAULT NULL::text[], IN data_type text[] DEFAULT NULL::text[], IN fkeys text[] DEFAULT NULL::text[], IN fdata_values text[] DEFAULT NULL::text[], IN _logo bytea DEFAULT NULL::bytea)
    language plpgsql
as
$$
DECLARE
    _id_attribute_name bigint;
    _temp_values text[];
    _id_attribute_value bigint;
    _temp_boolean BOOLEAN;

BEGIN

    -- Попытки вставки значений в поля таблицы vendors
    IF _name IS NOT NULL THEN
        UPDATE vendors SET name = _name WHERE id = _id;
    END IF;

    IF _flag_description IS TRUE THEN
        UPDATE vendors SET description = _description WHERE id = _id;
    END IF;

    IF _flag_link_to IS TRUE THEN
        UPDATE vendors SET link_to = _link_to WHERE id = _id;
    END IF;
    IF _flag_other_name IS TRUE THEN
        UPDATE vendors SET other_name = _other_name WHERE id = _id;
    END IF;
    IF _flag_public_name IS TRUE THEN
        UPDATE vendors SET public_name = _public_name WHERE id = _id;
    END IF;
    IF _flag_logo IS TRUE THEN
        UPDATE vendors SET logo = _logo WHERE id = _id;
    END IF;

    -- Обновление атрибутов
    IF keys IS NOT NULL AND data_values IS NOT NULL THEN
        IF array_length(keys, 1) != array_length(data_values, 1) THEN
            RAISE EXCEPTION 'Arrays keys and values must have the same length';
        END IF;

        FOR i IN array_lower(keys, 1) .. array_upper(keys, 1)
            LOOP
                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, id_scope)
                VALUES (keys[i], (SELECT id FROM guide_scope_attribute WHERE scope = 'Vendor'))
                ON CONFLICT (attribute_name) DO UPDATE
                    SET id_scope = CASE
                                       WHEN guide_attribute.id_scope <> (SELECT id FROM guide_scope_attribute WHERE scope = 'Vendor')
                                           THEN (SELECT id FROM guide_scope_attribute WHERE scope = 'Global')
                                       ELSE guide_attribute.id_scope
                        END
                RETURNING id INTO _id_attribute_name;

                -- Получение ID атрибута, независимо от того, был ли он только что создан или уже существовал
                SELECT id INTO _id_attribute_name FROM guide_attribute WHERE attribute_name = keys[i];

                -- Вставка данных в value_attribute в виде временного массива
                _temp_values[1] := data_values[i];

                IF data_values[i] IS NULL OR data_values[i] = 'null' THEN
                    -- Удалить запись из value_attribute, если data_values[i] равно NULL
                    DELETE FROM value_attribute
                    WHERE attribute_id = _id_attribute_name AND vendor_id = _id;
                ELSE
                    -- Проверить наличие записи в value_attribute для данного атрибута и программы
                    SELECT id INTO _id_attribute_value
                    FROM value_attribute
                    WHERE attribute_id = _id_attribute_name AND vendor_id = _id;

                    IF _id_attribute_value IS NULL THEN
                        -- Если записи нет, то добавить ее
                        INSERT INTO value_attribute (attribute_id, value, vendor_id, type_data)
                        VALUES (_id_attribute_name, _temp_values, _id, data_type[i]);
                    ELSE
                        -- В противном случае обновить данные в value_attribute
                        UPDATE value_attribute
                        SET value = _temp_values, type_data = data_type[i]
                        WHERE id = _id_attribute_value;
                    END IF;
                END IF;
            END LOOP;
    END IF;
    IF fkeys IS NOT NULL AND fdata_values IS NOT NULL THEN
        FOR i IN array_lower(fkeys, 1) .. array_upper(fkeys, 1) LOOP
                -- Преобразование текстового значения в булево
                _temp_boolean := CASE
                                     WHEN LOWER(fdata_values[i]) = 'true' OR fdata_values[i] = '1' THEN true
                                     ELSE false
                    END;

                -- Попытка вставки атрибута, игнорируя дубликаты
                INSERT INTO guide_attribute (attribute_name, filterable_attribute)
                VALUES (fkeys[i], _temp_boolean)
                ON CONFLICT (attribute_name) DO UPDATE
                    SET filterable_attribute = EXCLUDED.filterable_attribute
                RETURNING id INTO _id_attribute_name;
            END LOOP;
    END IF;
END;
$$;

alter procedure "UpdateVendor"(boolean, boolean, boolean, boolean, boolean, bigint, text, text, text, text, text, text[], text[], text[], text[], text[], bytea) owner to postgres;

create procedure delete_old_entities()
    language plpgsql
as
$$
BEGIN
    DELETE FROM filtershortlinks
    WHERE add_date < now() - interval '90 days';
END
$$;

alter procedure delete_old_entities() owner to postgres;

create function try_add_filter_shortlink_data(_number_unique_code integer, _hash_data character varying, _json_data json) returns integer
    language plpgsql
as
$$
	DECLARE existing_numcode INTEGER;
BEGIN
	call delete_old_entities();

        SELECT unique_numcode
        INTO existing_numcode
        FROM filtershortlinks
        WHERE hash_json_data = _hash_data;

        IF NOT FOUND THEN
            INSERT INTO filtershortlinks (unique_numcode, hash_json_data, source_json, add_date)
            VALUES (_number_unique_code, _hash_data, _json_data, CURRENT_TIMESTAMP)
            RETURNING unique_numcode INTO existing_numcode;
        END IF;

        RETURN existing_numcode;
END;
$$;

alter function try_add_filter_shortlink_data(integer, varchar, json) owner to postgres;

create function try_get_filter_shortlink_data(_num_code integer) returns character varying
    language plpgsql
as
$$
    DECLARE found_json_data CHARACTER VARYING;
    BEGIN
        SELECT source_json
        INTO found_json_data
        FROM filtershortlinks
        WHERE unique_numcode = _num_code;

        IF found_json_data IS NOT NULL THEN
            UPDATE filtershortlinks
            SET add_date = CURRENT_TIMESTAMP
            WHERE unique_numcode = _num_code;
        END IF;

        RETURN found_json_data;
END;
$$;

alter function try_get_filter_shortlink_data(integer) owner to postgres;