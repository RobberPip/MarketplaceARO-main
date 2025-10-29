# Logging

_На данный момент_ у логов такой формат:

```js
{
  trace_id: @tr,
  timestamp: @t,
  message: @m,
  level: @l,
  git: {
    commit: @p['GitCommit'],
    branch: @p['GitBranch']
  },
  program: {
    pid: @p['ProcessId'],
    name: @p['ProcessName']
  },
  fields: {
    @x, ..Rest()
  }
}
```

Как устроены:

- **systemd** сервис какает своими stdout в syslog
- **rsyslog** складывает логи приложения в /var/logs/market/\*.log
- за ротацию логов отвечает **logrotate**

```
ssh <user_name>@rds.arodevelop.ru -p 52 'cat /var/log/market/market.log'
```

## Lnav (kibana at home)

В качестве инструмента для работы с логами можно использовать [lnav](https://github.com/tstack/lnav).

Страшненький TUI, но в плане функциональности можно получить все то же что есть в kibana.

```bash
brew install lnav

# Обновить/добавить наш формат лога в lnav
cp docs/logging/lnav-format.json ~/.lnav/formats/installed/dotnet.json

# загрузить логи в lnav
ssh <user_name>@rds.arodevelop.ru -p 52 'cat -v /var/log/market/market.log' | lnav
```

в [docs/logging/lnav-format.json](./lnav-format.json) описана схема наших логов, для того что-бы lnav знал какие поля можно ожидать (`.value`) и как именно отображать данные построчно (`.["line-format"]`)

Фильтровать логи можно как специальными командами (`:filter-expr`), так и перейдя в режим sqlite (`;`):

```sql
SELECT * FROM market_app WHERE trace_id = 'f4e28249a68afb7fb624ee67b5bc5d5f'
```
