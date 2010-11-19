PostLog
---

PostLog is an appender for log4net designed to submit log messages via HTTP.

Configuration
---

PostLog's `HttpAppender` is configured in the `log4net` app.config section.
Several options are available.

- `uri` The location to send the HTTP Request
- `method` The HTTP method to be used (defaults to POST)
- `useragent` The value of the useragent header
- `format` The format to serialize the log data. Options are *form*, *json*, and *xml*
- `xmlrootname` root element tag for xml

Also, you must add `parameter` tags for each value that is sent.

Example
---

    <log4net>
      <appender name="HttpAppender" type="PostLog.HttpAppender">
        <uri value="http://localhost:34343/log" />
        <parameter>
          <name value="date" />
          <layout type="log4net.Layout.RawTimeStampLayout" />
        </parameter>
        <parameter>
          <name value="level" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%level" />
          </layout>
        </parameter>
        <parameter>
          <name value="message" />
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%message" />
          </layout>
        </parameter>
        <parameter>
          <name value="exception" />
          <layout type="log4net.Layout.ExceptionLayout" />
        </parameter>
      </appender>
      <root>
        <level value="DEBUG" />
        <appender-ref ref="HttpAppender" />
      </root>
    </log4net>
  </configuration>


