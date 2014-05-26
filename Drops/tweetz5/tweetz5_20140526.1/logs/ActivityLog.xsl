<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" encoding="utf-8" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" />
  <xsl:template match="BuildInformation">
    <head>
      <title _locID="LogTitle">Build Process Activity Log</title>
      <style type="text/css">
        body {
          font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
          font-size: 0.7em;
          margin: 0 0.5em;
        }
        div.node {
          margin-top: 0.1em;
          padding: 0;
        }
        div.node div.node {
          margin-left: 1.5em;
        }
        .name, .duration, .message, .link {
          font-size: 1.2em;
        }
        div.header {
          overflow: auto; 
          border-top: 1px solid #DDD;
        }
        div.header span.name {
          float: left;
        }
        div.header span.duration {
          float: right; 
          padding-right: 1em;
        }
        div.properties {
          font-size: 1em;
          margin-left: 1.5em;
          margin-top: 0;
          margin-bottom: 0;
          padding-top: 0;
          padding-bottom: 0;
        }
        p.message {
          margin: 0;
          word-wrap: break-word; 
          width: 95%;
        }
        .dim {
          color: #888;
        }
        dl {
          margin: 0;
          padding-top: 0.2em;
        }
        dl dt {
          color: #888;
          margin-top: 0.1em;
          padding-top: 0;
          padding-bottom: 0;
        }
        dl dd {
        }
        dl dd ul {
          list-style: none;
          margin: 0;
          padding: 0;
          text-indent: -2.0em;
        }
        ul li {
          list-style-type: none;
          color: #888;
          margin: 0;
          padding: 0;
        }
        .link {
          padding-left: 1em;
        }
      </style>
    </head>
    <body>
      <xsl:apply-templates select="BuildInformationNode" />
    </body>
  </xsl:template>
  <xsl:template match="BuildInformationNode[@Type = 'ActivityTracking']">
    <div class="node">
      <div class="header">
        <span class="name"><xsl:value-of select="Fields/InformationField[@Name = 'DisplayName']/@Value" /></span>
        <span class="duration"><xsl:value-of select="Fields/InformationField[@Name = 'Duration']/@Value"/></span>    
      </div>
      <xsl:apply-templates select="Children/BuildInformationNode[@Type = 'ActivityInput']" />
      <xsl:apply-templates select="Children/BuildInformationNode[@Type = 'ActivityOutput']" />
      <xsl:apply-templates select="Children/BuildInformationNode[@Type != 'ActivityOutput' and @Type != 'ActivityInput']" />
    </div>
  </xsl:template>
  <xsl:template match="BuildInformationNode[@Type = 'AgentScopeActivityTracking']">
    <xsl:variable name="uri" select="Fields/InformationField[@Name = 'ReservedAgentUri']/@Value" />
    <xsl:variable name="name" select="Fields/InformationField[@Name = 'ReservedAgentName']/@Value" />
    <xsl:variable name="status" select="Fields/InformationField[@Name = 'ReservationStatus']/@Value" />
    <xsl:variable name="displayName" select="Fields/InformationField[@Name = 'DisplayName']/@Value" />
    <div class="node">
      <div class="header">
        <xsl:choose>
          <xsl:when test="$status = 'AgentRequested'">
            <span class="name" _locID="AgentRequested"><xsl:value-of select="$displayName" /> (waiting for a build agent)</span>
          </xsl:when>
          <xsl:when test="$status = 'AgentReserved' or $status = 'AgentReleased'">
            <span class="name" _locID="AgentReserved"><xsl:value-of select="$displayName"/> (reserved build agent <xsl:value-of select="$name" />)</span>
          </xsl:when>
        </xsl:choose>
        <span class="duration"><xsl:value-of select="Fields/InformationField[@Name = 'Duration']/@Value"/></span>
      </div>
      <xsl:apply-templates select="Children/BuildInformationNode[@Type = 'ActivityInput']" />
      <xsl:apply-templates select="Children/BuildInformationNode[@Type = 'ActivityOutput']" />
      <xsl:if test="starts-with($uri, 'vstfs:///Build/Agent/')">
        <xsl:variable name="agentLog" select="concat('ActivityLog.AgentScope.', substring-after($uri, 'vstfs:///Build/Agent/'), '.xml')" />
        <div class="node">
          <a class="link" _locID="ViewLog">
            <xsl:attribute name="href"><xsl:value-of select="$agentLog" /></xsl:attribute>
            <xsl:text>View log</xsl:text>
          </a>
        </div>
      </xsl:if>
      <xsl:apply-templates select="Children/BuildInformationNode[@Type != 'ActivityOutput' and @Type != 'ActivityInput']" />      
    </div>
  </xsl:template>
  <xsl:template match="BuildInformationNode[@Type = 'BuildError' or @Type = 'BuildMessage' or @Type = 'BuildWarning']">
    <div class="node">
      <p class="message"><xsl:value-of select="Fields/InformationField[@Name = 'Message']/@Value" /></p>
    </div>
  </xsl:template>
  <xsl:template match="BuildInformationNode[@Type = 'ActivityInput' or @Type = 'ActivityOutput']">
    <div class="properties">
      <dl>
        <xsl:choose>
          <xsl:when test="@Type = 'ActivityInput'">
            <dt _locID="ActivityInput">Inputs</dt>
          </xsl:when>
          <xsl:otherwise>
            <dt _locID="ActivityOutput">Outputs</dt>
          </xsl:otherwise>
        </xsl:choose>
        <dd>
          <ul>
            <xsl:for-each select="Fields/InformationField">
              <li><xsl:value-of select="@Name"/>: <xsl:value-of select="@Value" /></li>
            </xsl:for-each>
          </ul>
        </dd>
      </dl>
    </div>
  </xsl:template>
</xsl:stylesheet>
