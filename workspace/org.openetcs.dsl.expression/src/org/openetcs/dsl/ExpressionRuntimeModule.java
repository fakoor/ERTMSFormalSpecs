/*
 * generated by Xtext
 */
package org.openetcs.dsl;

import org.eclipse.xtext.scoping.IGlobalScopeProvider;
import org.openetcs.dsl.scoping.ExpressionGlobalScopeProvider;

/**
 * Use this class to register components to be used at runtime / without the Equinox extension registry.
 */
public class ExpressionRuntimeModule extends org.openetcs.dsl.AbstractExpressionRuntimeModule {

	@Override
	public Class<? extends IGlobalScopeProvider> bindIGlobalScopeProvider() {
		return ExpressionGlobalScopeProvider.class;
	}
}
