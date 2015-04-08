<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" omit-xml-declaration="yes" indent="yes"/>

  <xsl:variable name="ItemTypeConfigPath">Property|RelationshipType|View|Server Event|Item Action|ItemType Life Cycle|Allowed Workflow|TOC Access|TOC View|Client Event|Can Add|Allowed Permission|Item Report|Morphae</xsl:variable>
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="Item">
    <Item action="get">
      <xsl:copy-of select="@*[local-name() != 'action' and local-name() != 'levels']"/>

      <xsl:choose>
        <xsl:when test="@type = 'ItemType'">
          <xsl:attribute name="levels">2</xsl:attribute>
          <xsl:attribute name="config_path">
            <xsl:value-of select="$ItemTypeConfigPath"/>
          </xsl:attribute>
        </xsl:when>
        <xsl:when test="@type = 'Item Report'">
          <xsl:attribute name="levels">0</xsl:attribute>
          <xsl:attribute name="related_expand">0</xsl:attribute>
        </xsl:when>
        <xsl:when test="@type = 'RelationshipType'">
          <xsl:attribute name="levels">1</xsl:attribute>
          <relationship_id>
            <Item type="ItemType" action="get" levels="2">
              <xsl:attribute name="config_path">
                <xsl:value-of select="$ItemTypeConfigPath"/>
              </xsl:attribute>
            </Item>
          </relationship_id>
        </xsl:when>
        <xsl:when test="@type = 'Action'">
          <xsl:attribute name="levels">1</xsl:attribute>
          <method>
            <Item type="Method" action="get" select="config_id"></Item>
          </method>
          <on_complete>
            <Item type="Method" action="get" select="config_id"></Item>
          </on_complete>
        </xsl:when>
        <xsl:when test="@type = 'Report'">
          <xsl:attribute name="levels">1</xsl:attribute>
          <method>
            <Item type="Method" action="get" select="config_id"></Item>
          </method>
        </xsl:when>
        <xsl:when test="contains('|Identity|List|Team|Method|Permission|Sequence|UserMessage|Workflow Promotion|', concat('|', @type, '|'))">
          <xsl:attribute name="levels">1</xsl:attribute>
        </xsl:when>
        <xsl:when test="contains('|Grid|User|Preference|Property|', concat('|', @type, '|'))">
          <xsl:attribute name="levels">2</xsl:attribute>
        </xsl:when>
        <xsl:when test="contains('|Form|Life Cycle Map|Workflow Map|', concat('|', @type, '|'))">
          <xsl:attribute name="levels">3</xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="levels">1</xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>

      <xsl:apply-templates />
      <!--<xsl:choose>
        <xsl:when test="@id and @id != ''">
          <id condition="in">
            (select ic.id
            from innovator.[<xsl:value-of select="translate(@type, ' ', '_')" />] i
            INNER JOIN innovator.[<xsl:value-of select="translate(@type, ' ', '_')" />] ic
            on ic.CONFIG_ID = i.CONFIG_ID
            and ic.IS_CURRENT = 1
            where i.id = '<xsl:value-of select="@id"/>')
          </id>
        </xsl:when>
        <xsl:otherwise>
          
        </xsl:otherwise>
      </xsl:choose>-->
    </Item>
  </xsl:template>
</xsl:stylesheet>
