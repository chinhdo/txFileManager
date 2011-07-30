<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method='text' encoding='ISO-8859-1' />

  <xsl:template match="/">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="test-results">
    <xsl:apply-templates select="//test-case[failure]"/>
  </xsl:template>

  <xsl:template match="test-case">
    <xsl:value-of select="substring-before(substring-after(failure/stack-trace, ' in '), ':line')"/>
    <xsl:text>(</xsl:text>
    <xsl:value-of select="normalize-space(substring-after(substring-after(failure/stack-trace, ' in '), ':line '))"/>
    <xsl:text>)</xsl:text>
    <xsl:text> : warning NU001: </xsl:text>
    <xsl:value-of select="@name"/><xsl:text>: </xsl:text>
    <xsl:value-of select="normalize-space(child::node()/message)" />
    <xsl:text disable-output-escaping='yes'>&#xD;&#xA;</xsl:text>
  </xsl:template>
</xsl:stylesheet>