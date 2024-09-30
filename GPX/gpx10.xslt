<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:gpx="http://www.topografix.com/GPX/1/0"
                xmlns:groundspeak="http://www.groundspeak.com/cache/1/0/1"
                xmlns:mbp="http://www.idpf.org/2007/mbp"
                exclude-result-prefixes="msxsl">

  <xsl:output method="html"
              indent="yes"
              doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
              doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"/>

  <xsl:template match="gpx:gpx">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
        <title>
          <xsl:value-of select="gpx:desc"/> @ <xsl:value-of select="gpx:name"/>
        </title>
        <style type="text/css">
          .title {
          font-size: 16px;
          color: #333333;
          line-height: 18px;
          font-family: Verdana,Arial,Helvetica;
          text-align: left;
          text-indent: 0;
          }
          .subtitle {
          font-size: 14px;
          color: #333333;
          line-height: 14px;
          font-family: Verdana,Arial,Helvetica;
          text-align: left;
          text-indent: 0;
          }
          .subsubtitle {
          font-size: 12px;
          color: #333333;
          line-height: 14px;
          font-family: Verdana,Arial,Helvetica;
          text-align: left;
          text-indent: 0;
          }
          .text {
          font-size: 10px;
          color: #666666;
          line-height: 12px;
          font-family: Verdana,Arial,Helvetica;
          text-align: left;
          text-indent: 0;
          }
          .embedded {
          font-size: 8px;
          color: #666666;
          background-color: #cccccc;
          line-height: 12px;
          font-family: Verdana,Arial,Helvetica;
          text-align: left;
          text-indent: 0;
          }
        </style>
      </head>
      <body>
        <!-- general section -->
        <a id="start" name="start"/>
        <h1 class="title">
          <xsl:value-of select="gpx:desc"/>
        </h1>
        <div class="text">
          Location: <xsl:value-of select="gpx:name"/>
        </div>
        <div class="text">
          Name: <xsl:value-of select="gpx:name"/>
        </div>
        <div class="text">
          Author: <xsl:value-of select="gpx:author"/>
        </div>
        <div class="text">
          Date/Time: <xsl:value-of select="gpx:time"/>
        </div>
        <div class="text">
          <br/>
          Generated with GPX File Cleaner by Not Sure
        </div>
        <a class="text" href="https://gpxfilecleaner.codeplex.com/">
          GPX File Cleaner @ CodePlex
        </a>
        <mbp:pagebreak/>
        <!-- table of content -->
        <a id="TOC" name="TOC"/>
        <h2 class="subtitle">
          Table of Content
        </h2>
        <ul>
          <li>
            <a href="#traditional">Traditional Caches</a>
          </li>
          <li>
            <a href="#multi">Multi Stage Caches</a>
          </li>
          <li>
            <a href="#mystery">Mystery Caches</a>
          </li>
          <li>
            <a href="#other">Other Cache Types</a>
          </li>
        </ul>
        <!-- page for each cache -->
        <xsl:call-template name="traditional"/>
        <xsl:call-template name="multi"/>
        <xsl:call-template name="mystery"/>
        <xsl:call-template name="other"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="traditional">
    <mbp:pagebreak/>
    <a id ="traditional" name="#traditional"/>
    <h2 class="subtitle">
      Traditional Caches
    </h2>
    <xsl:for-each select="gpx:wpt">
      <xsl:if test="gpx:type='Geocache|Traditional Cache'">
        <xsl:call-template name="wpt"/>
        <xsl:for-each select="groundspeak:cache">
          <xsl:call-template name="cache"/>
        </xsl:for-each>
        <mbp:pagebreak/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="multi">
    <mbp:pagebreak/>
    <a id ="multi" name="#multi"/>
    <h2 class="subtitle">
      Multi Caches
    </h2>
    <xsl:for-each select="gpx:wpt">
      <xsl:if test="gpx:type='Geocache|Multi-cache'">
        <xsl:call-template name="wpt"/>
        <xsl:for-each select="groundspeak:cache">
          <xsl:call-template name="cache"/>
        </xsl:for-each>
        <mbp:pagebreak/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="mystery">
    <mbp:pagebreak/>
    <a id ="mystery" name="#mystery"/>
    <h2 class="subtitle">
      Mystery Caches
    </h2>
    <xsl:for-each select="gpx:wpt">
      <xsl:if test="gpx:type='Geocache|Unknown Cache'">
        <xsl:call-template name="wpt"/>
        <xsl:for-each select="groundspeak:cache">
          <xsl:call-template name="cache"/>
        </xsl:for-each>
        <mbp:pagebreak/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="other">
    <mbp:pagebreak/>
    <a id ="other" name="#other"/>
    <h2 class="subtitle">
      Other Cache Types
    </h2>
    <xsl:for-each select="gpx:wpt">
      <xsl:if test="gpx:type!='Geocache|Traditional Cache'">
        <xsl:if test="gpx:type!='Geocache|Multi-cache'">
          <xsl:if test="gpx:type!='Geocache|Unknown Cache'">
            <xsl:call-template name="wpt"/>
            <xsl:for-each select="groundspeak:cache">
              <xsl:call-template name="cache"/>
            </xsl:for-each>
            <mbp:pagebreak/>
          </xsl:if>
        </xsl:if>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="wpt" match="gpx:wpt">
    <h2 class="subtitle">
      <div>
        <xsl:choose>
          <xsl:when test="gpx:sym = 'Geocache Found'">
            <span style="text-decoration:line-through;">Cache</span>
          </xsl:when>
          <xsl:otherwise>
            <span style="text-decoration:none;">Cache</span>
          </xsl:otherwise>
        </xsl:choose>
        &#160;<xsl:value-of select="gpx:name"/>
      </div>
      <xsl:element name="a">
        <xsl:attribute name="href">
          <xsl:value-of select="gpx:url"/>
        </xsl:attribute>
        <xsl:value-of select="gpx:urlname"/>
      </xsl:element>
    </h2>
    <p>
      <div class="text">
        <xsl:value-of select="gpx:desc"/>
      </div>
      <h3 class="subsubtitle">
        <span>
          <xsl:choose>
            <xsl:when test="@lat &lt; 0">
              S &#160;
              <xsl:value-of select="ceiling(@lat) * -1"/>'
              <xsl:value-of select="ceiling( ( @lat - ceiling(@lat) ) * -600000) div 10000"/>
            </xsl:when>
            <xsl:otherwise>
              N &#160;
              <xsl:value-of select="floor(@lat)"/>'
              <xsl:value-of select="floor( ( @lat - floor(@lat) ) * 600000) div 10000"/>
            </xsl:otherwise>
          </xsl:choose>
        </span>
        &#160;
        <span>
          <xsl:choose>
            <xsl:when test="@lon &lt; 0">
              W &#160;
              <xsl:value-of select="ceiling(@lon) * -1"/>'
              <xsl:value-of select="ceiling( ( @lon - ceiling(@lon) ) * -600000) div 10000"/>
            </xsl:when>
            <xsl:otherwise>
              E &#160;
              <xsl:value-of select="floor(@lon)"/>'
              <xsl:value-of select="floor( ( @lon - floor(@lon) ) * 600000) div 10000"/>
            </xsl:otherwise>
          </xsl:choose>
        </span>
      </h3>
    </p>
  </xsl:template>

  <xsl:template name="cache" match="groundspeak:cache">
    <p>
      <div class="text">
        Type: <xsl:value-of select="groundspeak:type"/>
      </div>
      <div class="text">
        Container: <xsl:value-of select="groundspeak:container"/>
      </div>
      <div class="text">
        Difficulty: <xsl:value-of select="groundspeak:difficulty"/>
      </div>
      <div class="text">
        Terrain: <xsl:value-of select="groundspeak:terrain"/>
      </div>
      <div class="text">
        Hint: <xsl:value-of select="groundspeak:encoded_hints"/>
      </div>
    </p>
    <p>
      <mbp:pagebreak/>
      <h3 class="subsubtitle">
        Short Description
      </h3>
      <div class="text">
        <xsl:value-of select="groundspeak:short_description" disable-output-escaping="yes"/>
      </div>
    </p>
    <p>
      <h3 class="subsubtitle">
        Long Description
      </h3>
      <div class="text">
        <xsl:value-of select="groundspeak:long_description" disable-output-escaping="yes"/>
      </div>
    </p>
  </xsl:template>

</xsl:stylesheet>
