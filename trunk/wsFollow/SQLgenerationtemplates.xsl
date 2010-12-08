<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:gen="http://www.codegeneration.net/" version="2.0">

    <!-- Builds the SQL model from the original data model -->
<xsl:template match="table" mode="sql">

DROP TABLE IF EXISTS <xsl:value-of select="@name" />;

CREATE <xsl:value-of select="@name" /> (

<xsl:apply-templates mode="sql" select="field" />

PRIMARY KEY ( <xsl:value-of select="@primary-key" /> )

        );

</xsl:template>

</xsl:stylesheet>

