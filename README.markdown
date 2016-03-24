PostLog
===

PostLog is an appender for log4net designed to submit log messages via HTTP.

Configuration
---

PostLog's `HttpAppender` is configured in the `log4net` app.config section.
Several options are available.

- `uri` The location to send the HTTP Request
- `method` The HTTP method to be used (defaults to POST)
- `useragent` The value of the useragent header
- `formatterType` The IBodyFormatter implementation to serialize logs

Also, you must add `parameter` tags for each value that is sent.

Example
---
```xml
    <log4net>
      <appender name="HttpAppender" type="PostLog.HttpAppender">
        <uri value="http://localhost:34343/log" />
        <formatterType value="PostLog.JsonBodyFormatter, PostLog"/>
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
```
License
---

The MIT License

Copyright (c) 2010-2011 Jason Staten

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.



[nugget]: http://nugget.codeplex.com/ 
