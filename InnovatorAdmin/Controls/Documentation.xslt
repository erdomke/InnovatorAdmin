<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" omit-xml-declaration="yes" standalone="yes" indent="yes"></xsl:output>

  <xsl:template match="doc/members">
    <FlowDocument>
      <xsl:for-each select="member[contains(@name, 'M:Aras.Tools.InnovatorAdmin.ArasXsltExtensions')]">
        <Section BorderBrush="Black" BorderThickness="0,0,0,1">
          <Paragraph TextAlignment="Left" FontSize="24" FontWeight="Bold">
            <xsl:value-of select="substring-after(@name, 'M:Aras.Tools.InnovatorAdmin.ArasXsltExtensions.')" />
          </Paragraph>
          <xsl:apply-templates select="summary" />
          <xsl:if test="param or returns">
            <Paragraph TextAlignment="Left" FontSize="20" FontWeight="Bold">Syntax</Paragraph>
            <xsl:if test="param">
              <Section>
                <Paragraph TextAlignment="Left"><Bold>Parameters</Bold></Paragraph>
                <xsl:apply-templates select="param" />
              </Section>
            </xsl:if>
            <xsl:apply-templates select="returns" />
          </xsl:if>
          <xsl:if test="exception">
            <Paragraph TextAlignment="Left" FontSize="16">Exceptions</Paragraph>
            <Table>
              <Table.Columns>
                <TableColumn/>
                <TableColumn/>
              </Table.Columns>
              <TableRowGroup>
                <TableRow Background="#FFDDDDDD" Foreground="#FF555555">
                  <TableCell>
                    <Paragraph TextAlignment="Left">Exception</Paragraph>
                  </TableCell>
                  <TableCell>
                    <Paragraph TextAlignment="Left">Condition</Paragraph>
                  </TableCell>
                </TableRow>
                <xsl:apply-templates select="exception" />    
              </TableRowGroup>
            </Table>
          </xsl:if>
        <xsl:apply-templates select="remarks" />
        </Section>
      </xsl:for-each>
    </FlowDocument>
  </xsl:template>

  <xsl:template match="c">
    <Bold><xsl:apply-templates /></Bold>
  </xsl:template>

  <xsl:template match="code">
    <Floater>
      <Paragraph TextAlignment="Left"><xsl:apply-templates mode="pre"/></Paragraph>
    </Floater>
  </xsl:template>

  <xsl:template match="example">
    <Paragraph TextAlignment="Left"><Bold>Example: </Bold></Paragraph>
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="exception">
    <TableRow>
      <TableCell>
        <Paragraph TextAlignment="Left"><Hyperlink><xsl:value-of select="substring-after(@cref,'T:')"/></Hyperlink></Paragraph>
      </TableCell>
      <TableCell>
        <Paragraph TextAlignment="Left"><xsl:apply-templates /></Paragraph>
      </TableCell>
    </TableRow>
  </xsl:template>

  <xsl:template match="include">
    <Hyperlink NavigateUri="{@file}">External file</Hyperlink>
  </xsl:template>

  <xsl:template match="para">
    <Paragraph TextAlignment="Left"><xsl:apply-templates /></Paragraph>
  </xsl:template>

  <xsl:template match="param">
    <Paragraph TextAlignment="Left" Margin="Auto,Auto,Auto,0">
      <Italic><xsl:value-of select="@name"/></Italic>
    </Paragraph>
    <Paragraph TextAlignment="Left" Margin="30,0,Auto,Auto">
      <xsl:apply-templates />
    </Paragraph>
  </xsl:template>

  <xsl:template match="paramref">
    <Italic><xsl:value-of select="@name" /></Italic>
  </xsl:template>

  <xsl:template match="permission">
    <Paragraph><Bold>Permission: </Bold><Italic><xsl:value-of select="@cref" /> </Italic></Paragraph>
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="remarks">
    <Paragraph TextAlignment="Left" FontSize="20" FontWeight="Bold">Remarks</Paragraph>
    <Paragraph><xsl:apply-templates /></Paragraph>
  </xsl:template>

  <xsl:template match="returns">
    <Paragraph TextAlignment="Left"><Bold>Return Value:</Bold></Paragraph>
    <Paragraph TextAlignment="Left"><xsl:apply-templates /></Paragraph>
  </xsl:template>

  <xsl:template match="see">
    <Hyperlink><xsl:value-of select="substring-after(@cref, ':')" /></Hyperlink>
  </xsl:template>

  <xsl:template match="seealso">
    <Hyperlink><xsl:value-of select="substring-after(@cref, ':')" /></Hyperlink>
  </xsl:template>
  
  <xsl:template match="summary">
    <Paragraph TextAlignment="Left"><xsl:apply-templates /></Paragraph>
  </xsl:template>

  <xsl:template match="text()">
    <xsl:param name="pText" select="."/>
    <xsl:copy-of select="$pText"/>
  </xsl:template>
    
  <xsl:template match="text()" name="insertBreaks" mode="pre">
     <xsl:param name="pText" select="."/>

     <xsl:choose>
       <xsl:when test="not(contains($pText, '&#xA;'))">
         <xsl:copy-of select="$pText"/>
       </xsl:when>
       <xsl:otherwise>
         <xsl:value-of select="substring-before($pText, '&#xA;')"/>
         <LineBreak />
         <xsl:call-template name="insertBreaks">
           <xsl:with-param name="pText" select=
             "substring-after($pText, '&#xA;')"/>
         </xsl:call-template>
       </xsl:otherwise>
     </xsl:choose>
   </xsl:template>
</xsl:stylesheet>